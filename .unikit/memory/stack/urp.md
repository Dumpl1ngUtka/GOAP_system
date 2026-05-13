# Universal Render Pipeline (URP)

> **Scope**: Universal Render Pipeline (URP) configuration, performance optimization, custom Scriptable Renderer Features, and shader development conventions.
> **Load when**: configuring graphics settings, optimizing rendering performance, authoring custom render passes or shaders, setting up post-processing volumes, or debugging rendering issues in URP.

---

## Core Concepts

- **Scriptable Render Pipeline (SRP)**: URP is a prebuilt SRP that allows for highly optimized graphics across a wide range of platforms.
- **URP Asset**: The primary configuration file containing global settings like shadow distance, MSAA, and HDR.
- **Renderer Data**: Defines the rendering path (Forward/Deferred) and contains the list of **Scriptable Renderer Features**.
- **SRP Batcher**: A rendering loop that speeds up your CPU rendering by reducing the overhead of material data updates. It batches draw calls that use the same shader variant.

## API / Interface

### Scriptable Renderer Feature
Use `ScriptableRendererFeature` to inject custom passes into the URP pipeline.

```csharp
public class CustomRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private LayerMask layerMask;
    private CustomRenderPass customPass;

    public override void Create()
    {
        customPass = new CustomRenderPass(layerMask);
        // Configures where the render pass should be injected.
        customPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customPass);
    }
}
```

### Scriptable Render Pass
Defines the execution logic for a custom rendering step. For modern URP (17+), prefer using the **Render Graph** API.

```csharp
class CustomRenderPass : ScriptableRenderPass
{
    private LayerMask mask;

    public CustomRenderPass(LayerMask mask) => this.mask = mask;

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
    {
        // Modern URP Render Graph implementation
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("CustomPass", out var passData))
        {
            UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            
            builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => {
                // Rendering logic here
            });
        }
    }
}
```

## Best Practices

### Performance Optimization
- **Enable SRP Batcher**: This is the single most important optimization. Ensure custom shaders are SRP Batcher compatible by wrapping material properties in a `CBUFFER_START(UnityPerMaterial)` block.
- **Shader Stripping**: Configure shader stripping in `Project Settings > Graphics` to reduce build size and load times by removing unused variants (e.g., Fog, Instancing if not used).
- **Mobile Specifics**:
    - Use **FXAA** or **SMAA** instead of MSAA to save significant GPU bandwidth.
    - Disable **HDR** if bloom or high-precision colors are not required.
    - Reduce **Render Scale** to 0.8x or 0.9x on high-DPI mobile devices for a large FPS boost with minimal visual impact.
    - Limit **Shadow Cascades** to 1 or 2 and set a reasonable **Shadow Distance** (e.g., 50m).
- **Texture Compression**: Use **ASTC** for modern mobile devices or **ETC2** for older Android compatibility.

### Shading
- **Prefer Simple Lit**: For non-hero objects, use `Universal Render Pipeline/Simple Lit` or `Baked Lit` instead of the standard `Lit` shader.
- **Avoid Complex Lit on Mobile**: It is designed for high-end PC/Console and is too heavy for mobile.

## Common Pitfalls

- **Breaking SRP Batcher**: Using `[PerRendererData]` or changing shader keywords per-material can break batching. Batching occurs per **Shader Variant**.
- **Unnecessary Textures**: Enabling **Depth Texture** or **Opaque Texture** in the URP Asset when no shaders actually use them creates unnecessary overhead.
- **Overdraw**: Excessive use of transparent objects or complex alpha-testing can lead to overdraw bottlenecks, especially on mobile. Use the **Rendering Debugger** (Ctrl+Backspace) to inspect overdraw.
- **Shadow Projection**: High-resolution shadows on additional lights can drastically drop performance. Disable **Cast Shadows** on additional lights whenever possible.

## Debugging Workflow

1. **Frame Debugger**: Use it to identify why batches are broken (e.g., "Different Shader Keywords").
2. **Rendering Debugger**: Use the URP Runtime UI to toggle features like shadows or post-processing to isolate performance bottlenecks.
3. **Unity Profiler**: Monitor `RenderLoop.Draw` to understand CPU rendering costs.
