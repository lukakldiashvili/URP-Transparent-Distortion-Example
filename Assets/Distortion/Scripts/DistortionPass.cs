using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class DistortionPass : ScriptableRenderPass {
	private const string k_GlobalFullSceneColorTexture = "_GlobalFullSceneColorTexture";

	private PassData m_PassData;

	private RTHandle m_tmpRT;

	public void Setup(Action<PassData> passDataOptions, in RenderingData renderingData) {
		m_PassData ??= new PassData();

		passDataOptions?.Invoke(m_PassData);
	}

	public void Dispose() {
		m_tmpRT?.Release();
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
		ExecutePass(m_PassData, ref renderingData, ref context);
	}

	private static void ExecutePass(PassData passData, ref RenderingData renderingData,
	                                ref ScriptableRenderContext context) {
		if (renderingData.cameraData.isPreviewCamera)
			return;

		// if is scene camera and we want to disable in scene view
		if (renderingData.cameraData.isSceneViewCamera && passData.disableInSceneView)
			return;

		CommandBuffer cmd =
			#if UNITY_2022_1_OR_NEWER
			renderingData.commandBuffer;
			#else
			CommandBufferPool.Get();
		#endif

		var cameraData = renderingData.cameraData;

		using (new ProfilingScope(cmd, passData.profilingSampler)) {
			ProcessEffect(ref context);
		}


		void ProcessEffect(ref ScriptableRenderContext context) {
			var source =
				#if UNITY_2022_1_OR_NEWER
					passData.isBeforeTransparents
					? cameraData.renderer.GetCameraColorBackBuffer(cmd)
					: cameraData.renderer.cameraColorTargetHandle;
				#else
				cameraData.renderer.cameraColorTarget;
			#endif

			cmd.SetGlobalTexture(k_GlobalFullSceneColorTexture, source);

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}
	}

	internal class PassData {
		internal bool disableInSceneView;

		public ProfilingSampler profilingSampler;
	}
}