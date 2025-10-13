using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Planner
{
    public class GoapPlanner : IGoapPlanner
    {
        private class FactComparer : IEqualityComparer<Fact>
        {
            public bool Equals(Fact x, Fact y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                if (x.Tag != y.Tag) return false;
                if (x.Object == null) return y.Object == null;
                return x.Object.Equals(y.Object);
            }

            public int GetHashCode(Fact obj)
            {
                unchecked
                {
                    return ((int)obj.Tag * 397) ^ (obj.Object?.GetHashCode() ?? 0);
                }
            }
        }

        private class WorldState
        {
            private static readonly FactComparer Comparer = new FactComparer();

            public readonly HashSet<Fact> Facts;

            public WorldState(IEnumerable<Fact> facts)
            {
                Facts = new HashSet<Fact>(facts, Comparer);
            }

            public bool Satisfies(IEnumerable<FactWithCondition> desiredConditions)
            {
                foreach (var cond in desiredConditions)
                {
                    if (cond.Type == FactCondition.None)
                    {
                        continue;
                    }

                    bool has = Facts.Contains(cond.Fact);

                    if (cond.Type == FactCondition.Include && !has)
                    {
                        return false;
                    }

                    if (cond.Type == FactCondition.Exclude && has)
                    {
                        return false;
                    }
                }

                return true;
            }

            public WorldState ApplyAction(IGoapAction action)
            {
                var newFacts = new HashSet<Fact>(Facts, Comparer);
                foreach (var eff in action.GetEffects())
                {
                    if (eff.Type == FactCondition.Include)
                    {
                        newFacts.Add(eff.Fact);
                    }
                    else if (eff.Type == FactCondition.Exclude)
                    {
                        newFacts.Remove(eff.Fact);
                    }
                }

                return new WorldState(newFacts);
            }

            public bool CanApply(IGoapAction action)
            {
                foreach (var pre in action.GetPreconditions())
                {
                    if (!Facts.Contains(pre))
                    {
                        return false;
                    }
                }

                return action.CheckProceduralPrecondition(Facts);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is WorldState other))
                {
                    return false;
                }

                return Facts.SetEquals(other.Facts);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 0;
                    foreach (var fact in Facts)
                    {
                        hash ^= Comparer.GetHashCode(fact);
                    }

                    return hash;
                }
            }
        }

        private class Node
        {
            public WorldState State { get; set; }
            public float G { get; set; }
            public float H { get; set; }
        }

        private class MinPriorityQueue
        {
            private readonly List<(float priority, Node node)> heap = new List<(float, Node)>();

            public int Count => heap.Count;

            public void Enqueue(Node node, float priority)
            {
                heap.Add((priority, node));
                int i = heap.Count - 1;
                while (i > 0)
                {
                    int parent = (i - 1) / 2;
                    if (heap[parent].priority <= heap[i].priority) break;
                    Swap(i, parent);
                    i = parent;
                }
            }

            public Node Dequeue()
            {
                if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
                var min = heap[0].node;
                heap[0] = heap[^1];
                heap.RemoveAt(heap.Count - 1);
                int i = 0;
                while (true)
                {
                    int left = 2 * i + 1;
                    int right = 2 * i + 2;
                    int smallest = i;
                    if (left < heap.Count && heap[left].priority < heap[smallest].priority)
                        smallest = left;
                    if (right < heap.Count && heap[right].priority < heap[smallest].priority)
                        smallest = right;
                    if (smallest == i) break;
                    Swap(i, smallest);
                    i = smallest;
                }

                return min;
            }

            private void Swap(int a, int b)
            {
                (heap[a], heap[b]) = (heap[b], heap[a]);
            }
        }

        public Queue<IGoapAction> Plan(
            IGoapKnowledge knowledge,
            IGoapGoal goal,
            IEnumerable<IGoapAction> availableActions)
        {
            if (TryPlan(knowledge, goal, availableActions, out var plan))
            {
                return plan;
            }

            return null;
        }

        public bool TryPlan(
            IGoapKnowledge knowledge,
            IGoapGoal goal,
            IEnumerable<IGoapAction> availableActions,
            out Queue<IGoapAction> plan)
        {
            plan = null;
            var currentFacts = knowledge.GetAllFacts();
            var startState = new WorldState(currentFacts);
            var desired = goal.GetDesiredState().ToList();

            if (startState.Satisfies(desired))
            {
                plan = new Queue<IGoapAction>();
                return true;
            }

            var actionsList = availableActions.ToList();
            float minCost = actionsList.Any(a => a.Cost > 0) ? actionsList.Where(a => a.Cost > 0).Min(a => a.Cost) : 1f;

            var openSet = new MinPriorityQueue();
            var closedSet = new HashSet<WorldState>();
            var cameFrom = new Dictionary<WorldState, (WorldState prev, IGoapAction action)>();
            var gScore = new Dictionary<WorldState, float> { [startState] = 0f };

            float Heuristic(WorldState state)
            {
                int unsatisfied = 0;
                foreach (var cond in desired)
                {
                    if (cond.Type == FactCondition.None)
                    {
                        continue;
                    }

                    bool has = state.Facts.Contains(cond.Fact);
                    if (cond.Type == FactCondition.Include && !has)
                    {
                        unsatisfied++;
                    }

                    if (cond.Type == FactCondition.Exclude && has)
                    {
                        unsatisfied++;
                    }
                }

                return unsatisfied * minCost;
            }

            var startNode = new Node { State = startState, G = 0f, H = Heuristic(startState) };
            openSet.Enqueue(startNode, startNode.G + startNode.H);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                if (!closedSet.Add(current.State))
                {
                    continue;
                }

                if (current.State.Satisfies(desired))
                {
                    var path = new List<IGoapAction>();
                    var curState = current.State;
                    while (cameFrom.TryGetValue(curState, out var from))
                    {
                        path.Add(from.action);
                        curState = from.prev;
                    }

                    path.Reverse();
                    plan = new Queue<IGoapAction>(path);
                    return true;
                }

                foreach (var action in actionsList)
                {
                    if (!current.State.CanApply(action))
                    {
                        continue;
                    }

                    var newState = current.State.ApplyAction(action);
                    var tentativeG = current.G + action.Cost;

                    if (gScore.TryGetValue(newState, out var oldG) && tentativeG >= oldG)
                    {
                        continue;
                    }

                    gScore[newState] = tentativeG;
                    var h = Heuristic(newState);
                    cameFrom[newState] = (current.State, action);
                    var newNode = new Node { State = newState, G = tentativeG, H = h };
                    openSet.Enqueue(newNode, tentativeG + h);
                }
            }

            return false;
        }
    }
}