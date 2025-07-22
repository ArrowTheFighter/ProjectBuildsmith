using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StencilMaskFeature : ScriptableRendererFeature
{
    class StencilMaskPass : ScriptableRenderPass
    {
        private Material material;
        private Mesh mesh;
        private Transform targetTransform;

        public StencilMaskPass(Material mat, Mesh m, Transform t)
        {
            material = mat;
            mesh = m;
            targetTransform = t;
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material || !mesh || !targetTransform) return;

            CommandBuffer cmd = CommandBufferPool.Get("Stencil Mask Pass");

            // ✅ Use the live transform matrix every frame
            cmd.DrawMesh(mesh, targetTransform.localToWorldMatrix, material);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [Header("Stencil Volume Settings")]
    public Material stencilMaterial;
    public Mesh stencilMesh;
    public Transform stencilTransform;

    private StencilMaskPass stencilPass;

    public override void Create()
    {
        // ✅ Just pass the actual Transform here
        stencilPass = new StencilMaskPass(stencilMaterial, stencilMesh, stencilTransform);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (stencilPass != null)
            renderer.EnqueuePass(stencilPass);
    }
}
