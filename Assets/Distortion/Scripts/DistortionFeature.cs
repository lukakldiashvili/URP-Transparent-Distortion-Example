using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class DistortionFeature : ScriptableRendererFeature {
	public enum InjectionPoint {
		BeforeRenderingTransparents = RenderPassEvent.BeforeRenderingTransparents,
		BeforeRenderingPostProcessing = RenderPassEvent.BeforeRenderingPostProcessing,
		AfterRenderingPostProcessing = RenderPassEvent.AfterRenderingPostProcessing
	}

	[Tooltip("Do not change from BeforeRenderingPostProcessing unless you know what you're doing.")]
	public InjectionPoint injectionPoint = InjectionPoint.BeforeRenderingPostProcessing;

	[Space]
	[Tooltip("Effect Quality")]
	[Range(1f, 10f)] public float downSample = 1.0f;
	
	// Hidden by scope because of no need
	private ScriptableRenderPassInput requirements = ScriptableRenderPassInput.Color;

	// Hidden by scope because of incorrect behaviour in the editor
	private bool disableInSceneView = true;

	private DistortionPass fullScreenPass;
	private bool injectedBeforeTransparents;

	/// <inheritdoc/>
	public override void Create() {
		fullScreenPass                 = new DistortionPass();
		fullScreenPass.renderPassEvent = (RenderPassEvent) injectionPoint;

		ScriptableRenderPassInput modifiedRequirements = requirements;

		fullScreenPass.ConfigureInput(modifiedRequirements);
	}

	/// <inheritdoc/>
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
		fullScreenPass.Setup(SetupPassData, downSample, renderingData);

		renderer.EnqueuePass(fullScreenPass);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing) {
		fullScreenPass.Dispose();
	}

	void SetupPassData(DistortionPass.PassData passData) {
		passData.profilingSampler     ??= new UnityEngine.Rendering.ProfilingSampler("FullscreenDistortionPass");
		passData.disableInSceneView   =   disableInSceneView;
	}
}