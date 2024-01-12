using Server.Reawakened.Entities.AIAction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.Reawakened.Entities.AIBehavior;
public class AIBehavior_ComeBack : AIBaseBehavior
{
    private float ComeBack_MoveSpeed;
    private float _fromPosX;
    private float _fromPosY;
    private float _toPosX;
    private float _toPosY;
    private float _behaviorDuration;
    private float _behaviorStartTime;
    private AI_Action_GoTo _goTo;

    public AIBehavior_ComeBack(float moveSpeed)
    {
        ComeBack_MoveSpeed = moveSpeed;
    }

    public vector3 GetPositionToComeback()
    {
        return new vector3(_toPosX, _toPosY, 0f);
    }

    public override void Start(AIProcessData aiData, float startTime, string[] args)
    {
        _fromPosX = aiData.Sync_PosX;
        _fromPosY = aiData.Sync_PosY;
        if (args != null && args.Length == 2)
        {
            _toPosX = float.Parse(args[0], CultureInfo.InvariantCulture);
            _toPosY = float.Parse(args[1], CultureInfo.InvariantCulture);
        }
        float num = (float)Math.Sqrt((_toPosX - _fromPosX) * (_toPosX - _fromPosX) + (_toPosY - _fromPosY) * (_toPosY - _fromPosY));
        _behaviorDuration = num / ComeBack_MoveSpeed;
        _behaviorDuration /= aiData.Sync_SpeedFactor;
        _behaviorStartTime = startTime;
        _goTo = new AI_Action_GoTo(ref aiData, _fromPosX, _fromPosY, _toPosX, _toPosY, _behaviorStartTime, _behaviorStartTime + _behaviorDuration, sinusMove: false);
    }

    public override bool Update(AIProcessData aiData, float clockTime)
    {
        float behaviorRatio = GetBehaviorRatio(aiData, clockTime);
        _goTo.Update(ref aiData, clockTime);
        if (behaviorRatio == 1f)
        {
            return false;
        }
        return true;
    }

    public override float GetBehaviorRatio(AIProcessData aiData, float clockTime)
    {
        float num = (clockTime - _behaviorStartTime) / _behaviorDuration;
        if (num > 1f)
        {
            num = 1f;
        }
        else if (num < 0f)
        {
            num = 0f;
        }
        return num;
    }

    private void ComputeComebackPosition(AIProcessData aiData, AI_Behavior toBehavior)
    {
        if (toBehavior != null)
        {
            toBehavior.GetComebackPosition(aiData, ref _toPosX, ref _toPosY);
            return;
        }
        _toPosX = aiData.Sync_PosX;
        _toPosY = aiData.Sync_PosY;
    }

    public bool MustDoComeback(AIProcessData aiData, AI_Behavior toBehavior)
    {
        ComputeComebackPosition(aiData, toBehavior);
        float num = (aiData.Sync_PosX - _toPosX) * (aiData.Sync_PosX - _toPosX) + (aiData.Sync_PosY - _toPosY) * (aiData.Sync_PosY - _toPosY);
        float num2 = 0.25f;
        if (num > num2 * num2)
        {
            return true;
        }
        return false;
    }
}
