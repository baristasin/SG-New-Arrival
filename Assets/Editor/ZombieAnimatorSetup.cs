using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public static class ZombieAnimatorSetup
{
    private const string OutputPath = "Assets/Game/Animations/Zombie/";

    [MenuItem("Tools/Create Zombie Animator")]
    public static void CreateZombieAnimator()
    {
        // Create empty animation clips
        var idleClip = CreateClip("Zombie_Idle", true);
        var runClip = CreateClip("Zombie_Run", true);
        var attackClip = CreateClip("Zombie_Attack", false);

        // Create Animator Controller
        var controller = AnimatorController.CreateAnimatorControllerAtPath(OutputPath + "ZombieAnimatorController.controller");

        // Add parameters
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        // Get base layer state machine
        var rootStateMachine = controller.layers[0].stateMachine;

        // Add states
        var idleState = rootStateMachine.AddState("Idle", new Vector3(200, 0, 0));
        idleState.motion = idleClip;

        var runState = rootStateMachine.AddState("Run", new Vector3(200, 100, 0));
        runState.motion = runClip;

        var attackState = rootStateMachine.AddState("Attack", new Vector3(450, 50, 0));
        attackState.motion = attackClip;

        // Default state = Idle
        rootStateMachine.defaultState = idleState;

        // Idle -> Run (IsRunning = true)
        var idleToRun = idleState.AddTransition(runState);
        idleToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        idleToRun.hasExitTime = false;
        idleToRun.duration = 0.1f;

        // Run -> Idle (IsRunning = false)
        var runToIdle = runState.AddTransition(idleState);
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        runToIdle.hasExitTime = false;
        runToIdle.duration = 0.1f;

        // Any State -> Attack (Attack trigger)
        var anyToAttack = rootStateMachine.AddAnyStateTransition(attackState);
        anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        anyToAttack.hasExitTime = false;
        anyToAttack.duration = 0.1f;

        // Attack -> Idle (exit time)
        var attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 1f;
        attackToIdle.duration = 0.1f;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Zombie Animator Controller created at: " + OutputPath);
    }

    private static AnimationClip CreateClip(string name, bool loop)
    {
        var clip = new AnimationClip();
        clip.name = name;

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, OutputPath + name + ".anim");
        return clip;
    }
}
