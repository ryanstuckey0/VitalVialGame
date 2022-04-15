using ViralVial.Utilities;

namespace ViralVial.Player
{
    public class PermittedActions
    {
        private PermittedActionsState savedPermittedActionsState;

        private bool locked = false;

        private bool move = true;
        public bool Move
        {
            get { return move; }
            set
            {
                if (locked || value == move) return;
                move = value;
                EventManager.Instance.InvokeEvent(move ? "PlayerAllowMove" : "PlayerDisallowMove");
            }
        }

        private bool dash = true;
        public bool Dash
        {
            get { return dash; }
            set
            {
                if (locked || value == dash) return;
                dash = value;
                EventManager.Instance.InvokeEvent(dash ? "PlayerAllowDash" : "PlayerDisallowDash");
            }
        }

        private bool useHotbarSlots = true;
        public bool UseHotbarSlots
        {
            get { return useHotbarSlots; }
            set
            {
                if (locked || value == useHotbarSlots) return;
                useHotbarSlots = value;
                EventManager.Instance.InvokeEvent(useHotbarSlots ? "PlayerAllowUseHotbarSlots" : "PlayerDisallowUseHotbarSlots");
            }
        }

        private bool fire = true;
        public bool Fire
        {
            get { return fire; }
            set
            {
                if (locked || value == useThrowable) return;
                fire = value;
                EventManager.Instance.InvokeEvent(fire ? "PlayerAllowFire" : "PlayerDisallowFire");
            }
        }

        private bool melee = true;
        public bool Melee
        {
            get { return melee; }
            set
            {
                if (locked || value == melee) return;
                melee = value;
                EventManager.Instance.InvokeEvent(melee ? "PlayerAllowMelee" : "PlayerDisallowMelee");
            }
        }

        private bool reload = true;
        public bool Reload
        {
            get { return reload; }
            set
            {
                if (locked || value == reload) return;
                reload = value;
                EventManager.Instance.InvokeEvent(reload ? "PlayerAllowReload" : "PlayerDisallowReload");
            }
        }

        private bool useThrowable = true;
        public bool UseThrowable
        {
            get { return useThrowable; }
            set
            {
                if (locked || value == useThrowable) return;
                useThrowable = value;
                EventManager.Instance.InvokeEvent(useThrowable ? "PlayerAllowUseThrowable" : "PlayerDisallowUseThrowable");
            }
        }

        private bool switchGuns = true;
        public bool SwitchGuns
        {
            get { return switchGuns; }
            set
            {
                if (locked || value == switchGuns) return;
                switchGuns = value;
                EventManager.Instance.InvokeEvent(switchGuns ? "PlayerAllowSwitchGuns" : "PlayerDisallowSwitchGuns");
            }
        }

        private bool moveAim = true;
        public bool MoveAim
        {
            get { return moveAim; }
            set
            {
                if (locked || value == moveAim) return;
                moveAim = value;
                EventManager.Instance.InvokeEvent(moveAim ? "PlayerAllowMoveAim" : "PlayerDisallowMoveAim");
            }
        }

        private bool dualWield = true;
        public bool DualWield
        {
            get { return dualWield; }
            set
            {
                if (locked || value == dualWield) return;
                dualWield = value;
                EventManager.Instance.InvokeEvent(dualWield ? "PlayerAllowDualWield" : "PlayerDisallowDualWield");
            }
        }

        public void OnDestroy() { }

        public bool CanLoadState()
        {
            return !locked;
        }

        public void LoadState(PermittedActionsState stateToLoad)
        {
            if (!CanLoadState()) return;

            Move = stateToLoad.Move;
            Dash = stateToLoad.Dash;
            UseHotbarSlots = stateToLoad.UseHotbarSlots;
            Fire = stateToLoad.Fire;
            Melee = stateToLoad.Melee;
            Reload = stateToLoad.Reload;
            UseThrowable = stateToLoad.UseThrowable;
            SwitchGuns = stateToLoad.SwitchGuns;
            MoveAim = stateToLoad.MoveAim;
        }

        public void LoadStateAndLock(PermittedActionsState stateToLoad)
        {
            if (!CanLoadState()) return;
            LoadState(stateToLoad);
            LockPlayer();
        }

        public void ChangeAll(bool permitted)
        {
            if (!CanLoadState()) return;
            Move = permitted;
            Dash = permitted;
            UseHotbarSlots = permitted;
            Fire = permitted;
            Melee = permitted;
            Reload = permitted;
            UseThrowable = permitted;
            SwitchGuns = permitted;
            MoveAim = permitted;
        }

        /// <summary>
        /// Only to be used when absolutely necessary.
        /// </summary>
        /// <param name="locked">true to lock state of player, else false</param>
        public void LockPlayer()
        {
            locked = true;
        }

        public void UnlockPlayer()
        {
            locked = false;
        }
    }

    public struct PermittedActionsState
    {
        public bool Move;
        public bool Dash;
        public bool UseHotbarSlots;
        public bool Fire;
        public bool Melee;
        public bool Reload;
        public bool UseThrowable;
        public bool SwitchGuns;
        public bool MoveAim;
    }
}
