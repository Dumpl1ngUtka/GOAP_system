using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.Cards;
using Services.GameCard;

namespace Services.GameControl
{
    public class GameControlService : IDisposable
    {
        public event Action SpawnCard;
        public CardType CurrentCardType { get; private set; } = CardType.Duck;

        private CancellationTokenSource _cts;
        private readonly float _spawnInterval = 2f;

        public void StartGameSession()
        {
            StopGameSession();
            _cts = new CancellationTokenSource();
            GameLoop(_cts.Token).Forget();
        }

        public void StopGameSession()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async UniTaskVoid GameLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_spawnInterval), cancellationToken: token);
                
                SpawnCard?.Invoke();
                CurrentCardType++;
                
                if (!Enum.IsDefined(typeof(CardType), CurrentCardType) || CurrentCardType == CardType.None)
                {
                    CurrentCardType = CardType.Duck;
                }
            }
        }

        public void Dispose()
        {
            StopGameSession();
        }
    }
}