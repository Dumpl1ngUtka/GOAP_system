# MCP for Unity (CoplayDev)

> **Scope**: MCP for Unity bridge by CoplayDev — exposing custom C# tools to AI agents, scene and asset manipulation via MCP, bridge configuration, and tool authoring patterns.
> **Load when**: creating custom MCP tools for AI, exposing project-specific methods to Claude/Cursor/Copilot, configuring the Unity-MCP bridge, or debugging tool discovery and execution.

---

## Tool Authoring

Custom tools allow AI agents (like Claude or Cursor) to execute project-specific logic. Tools must be defined in **static classes** located within an `Editor/` folder.

### Basic Tool Structure

Use the `[McpForUnityTool]` attribute to register a tool. Every tool must implement a `HandleCommand` method.

```csharp
using Newtonsoft.Json.Linq;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Tools;

[McpForUnityTool("my_custom_tool", Description = "Describe what this tool does for the AI")]
public static class MyCustomTool
{
    // Synchronous handler
    public static object HandleCommand(JObject @params)
    {
        var parameters = @params.ToObject<Parameters>();
        
        // Implementation logic
        
        return new SuccessResponse("Execution successful", new { result = "data" });
    }

    // Parameters class for structured input
    public class Parameters
    {
        [ToolParameter("Description of param1")]
        public string param1 { get; set; }

        [ToolParameter("Optional parameter", Required = false)]
        public int? param2 { get; set; }
    }
}
```

### Async Tool Handler

For long-running operations (like building, baking, or complex searches), use an `async` handler.

```csharp
public static async Task<object> HandleCommand(JObject @params)
{
    await SomeLongTask();
    return new SuccessResponse("Finished long task");
}
```

---

## Response Types

The `HandleCommand` method should return one of the following types from `MCPForUnity.Editor.Helpers`:

| Type | Usage |
|------|-------|
| `SuccessResponse` | Tool finished successfully. Can include data. |
| `ErrorResponse` | Tool failed. Includes an error message. |
| `PendingResponse` | For long-running tools that require polling. |

---

## Long-Running Tasks & Polling

If a tool takes significant time, use the `RequiresPolling` property in the attribute and `McpJobStateStore` to manage state.

```csharp
[McpForUnityTool("bake_something", RequiresPolling = true, PollAction = "status")]
public static class BakeTool
{
    public static object HandleCommand(JObject @params)
    {
        // Start process and save initial state
        McpJobStateStore.SaveState("bake_something", new { progress = 0f });
        return new PendingResponse("Baking started...", 0.5); // 0.5s polling interval
    }

    public static object Status(JObject _)
    {
        var state = McpJobStateStore.LoadState<MyState>("bake_something");
        if (state.isDone) 
            return new { _mcp_status = "complete", data = state };
            
        return new PendingResponse($"Baking... {state.progress:P0}", 0.5, state);
    }
}
```

---

## Patterns & Best Practices

### "Find Then Read" Pattern
For efficiency, avoid reading full data for all objects at once.
1. Use a search tool (like `find_gameobjects`) to get IDs.
2. Use the `mcpforunity://scene/gameobject/{id}` resource URI to read detailed data only for relevant objects.

### Scene Manipulation
When modifying the scene from a tool, ensure you mark objects as dirty or record undo operations if intended for Editor use:
```csharp
Undo.RecordObject(target, "MCP Tool Modification");
target.value = newValue;
EditorUtility.SetDirty(target);
```

---

## CLI Usage

The Unity-MCP bridge can be queried via a CLI-like interface by the AI.

```bash
# Create a primitive
unity-mcp gameobject create "Cube" --primitive Cube --position 0 0 0

# Find and modify
unity-mcp gameobject find "Player"
unity-mcp gameobject modify "Cube" --position 10 0 0 --rotation 0 90 0
```
*Note: Multi-value options like `--position` use spaces as separators, not commas.*

---

## Anti-patterns

**Placing tool classes outside of `Editor/` folders.**
The bridge will not discover tools unless they are in an Editor-specific assembly or folder.

**Returning large, unoptimized data structures.**
LLM context windows are limited. Return only the data necessary for the AI to make its next decision. Use the "Find Then Read" pattern.

**Blocking the main thread with long-running synchronous handlers.**
Since most MCP operations happen on the Unity main thread (Editor context), long sync operations will freeze the Editor. Use `async` or polling for heavy tasks.

**Missing `[ToolParameter]` attributes.**
Without these, the AI won't know the purpose of the parameters or whether they are required, leading to incorrect tool calls.
