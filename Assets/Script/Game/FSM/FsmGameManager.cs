using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FSM
{
    public enum FsmState
    {
        /// <summary>
        /// 资源配置检测与更新
        /// </summary>
        FsmGameConfigState,
        /// <summary>
        /// 游戏初始化
        /// </summary>
        FsmGameInitState,
        /// <summary>
        /// 登录状态
        /// </summary>
        FsmGameLoginState,
    }

    public class FsmGameManager
    {
        private static FsmGameManager sInstance = null;
        
        #region Fsm Attribute

        public static FSM.FsmGameManager Instance
        {
            get {
                if (sInstance == null)
                    sInstance = new FsmGameManager();
                return sInstance; }
        }

        public FSM.FsmGameState Current
        {
            get { return current; }
        }

        public static void Create()
        {
            if (sInstance == null)
                sInstance = new FsmGameManager();
        }

        public static void Destroy()
        {

        }

        #endregion


        /// <summary>
        /// 当前状态
        /// </summary>
        private FsmGameState current;

        /// <summary>
        /// 状态列表
        /// </summary>
        private Dictionary<FsmState, FsmGameState> fsm;

        private FsmGameManager()
        {
            fsm = new Dictionary<FsmState, FsmGameState>();
        }

        /// <summary>
        /// 创建所有状态
        /// </summary>
        private void CreateAllState()
        {
            System.Type type = typeof(FsmState);
            System.Array allState = System.Enum.GetValues(type);

            int length = allState.Length;
            for (int i = 0; i < length; i++)
            {
                GetFsmInstance((FsmState)allState.GetValue(i));
            }
        }

        private FsmGameState GetFsmInstance(FsmState state)
        {
            if (fsm.ContainsKey(state))
                return fsm[state];

            FsmGameState newState = (FsmGameState)Assembly.Load("Assembly-CSharp").CreateInstance(string.Format("FSM.{0}",state.ToString()));
            fsm.Add(state, newState);

            //  once
            newState.Init();

            return newState;
        }

        public void ChangeState( FsmState state)
        {
            if (current != null)
            {
                current.Leave();
            }

            current = GetFsmInstance(state);
            current.Enter();
        }

        public void Update()
        {
            current.Update();
        }

        public void DestroyState( FsmState state)
        {
            if (fsm.ContainsKey(state))
            {
                //  销毁资源
                fsm[state].Destroy();
                fsm.Remove(state);
            }
        }

    }
}


