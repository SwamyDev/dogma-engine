using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Interfaces;
using Microsoft.Xna.Framework;
using Pontification.Monitoring;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    #region Ability state enumeration
    public enum AbilityState
    {
        AS_IDLE,
        AS_USING_PRIMARY,
        AS_USING_SECONDARY
    }
    #endregion
    public class AbilityManager : Component
    {
        #region Private attributes
        private AbilityState _state;
        private AbilityState _oldState;
        private List<IAbility> _primaryAbilities = new List<IAbility>();
        private List<IAbility> _secondaryAbilities = new List<IAbility>();
        private IAbility _activePrimary;
        private IAbility _activeSecondary;
        private int _idxPrimary;
        private int _idxSecondary;
        private bool _isLocked;
        #endregion

        #region Public proeprties
        public AbilityState State { get { return _state; } }
        public IAbility ActivePrimary { get { return _activePrimary; } }
        public IAbility ActiveSecondary { get { return _activeSecondary; } }
        public string UsingAbility
        {
            get
            {
                if (_state == AbilityState.AS_USING_PRIMARY)
                    return ActivePrimary.ToString();
                if (_state == AbilityState.AS_USING_SECONDARY)
                    return ActiveSecondary.ToString();

                return "";
            }
        }
        #endregion

        #region Delegates
        public delegate void AbilityStateChangedCallback(AbilityState newState, string animName);
        public AbilityStateChangedCallback StateChanged;
        #endregion

        #region Public methods
        public void ResetAbilityState()
        {
            if (!_isLocked)
            {
                _state = AbilityState.AS_IDLE;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_oldState != _state)
            {
                if (StateChanged != null)
                {
                    if (_state == AbilityState.AS_USING_PRIMARY)
                    {
                        StateChanged(_state, ActivePrimary.ToString());

                        if (GameObject == SceneInfo.Player)
                            Logger.Instance.Log(string.Format("Ability_{0}", ActivePrimary.ToString()), MessageType.MT_STATISTICS);
                    }
                    else if (_state == AbilityState.AS_USING_SECONDARY)
                    {
                        StateChanged(_state, ActiveSecondary.ToString());

                        if (GameObject == SceneInfo.Player)
                            Logger.Instance.Log(string.Format("Ability_{0}", ActiveSecondary.ToString()), MessageType.MT_STATISTICS);
                    }
                    else if (_state == AbilityState.AS_IDLE)
                    {
                        if (_oldState == AbilityState.AS_USING_PRIMARY)
                            StateChanged(_state, ActivePrimary.ToString());
                        else if (_oldState == AbilityState.AS_USING_SECONDARY)
                            StateChanged(_state, ActiveSecondary.ToString());
                        else
                            StateChanged(_state, "");

                        if (GameObject == SceneInfo.Player)
                            Logger.Instance.Log("Ability_None", MessageType.MT_STATISTICS);
                    }
                }
                _oldState = _state;
            }
        }

        public void AddAbility(IAbility ability, bool isSecondary = false)
        {
            if (isSecondary)
            {
                if (_activeSecondary == null)
                {
                    _activeSecondary = ability;
                    _idxSecondary = 0;
                }
                _secondaryAbilities.Add(ability);
                _secondaryAbilities.GetEnumerator();
            }
            else
            {
                if (_activePrimary == null)
                {
                    _activePrimary = ability;
                    _idxPrimary = 0;
                }
                _primaryAbilities.Add(ability);
            }
        }

        public void RemoveAbility(IAbility ability)
        {
            if (_primaryAbilities.Remove(ability))
            {
                if (_activePrimary == ability)
                    NextAbility(false);
            }
            else if (_secondaryAbilities.Remove(ability))
            {
                if (_activeSecondary == ability)
                    NextAbility(true);
            }
        }

        public void NextAbility(bool secondary = false)
        {
            if (secondary)
            {
                _idxSecondary = (_idxSecondary + 1) % _secondaryAbilities.Count;
                _activeSecondary = _secondaryAbilities[_idxSecondary];
            }
            else
            {
                _idxPrimary = (_idxPrimary + 1) % _primaryAbilities.Count;
                _activePrimary = _primaryAbilities[_idxPrimary];
            }
        }

        public void PreviousAbility(bool secondary = false)
        {
            if (secondary)
            {
                _idxSecondary = (_idxSecondary - 1 + _secondaryAbilities.Count) % _secondaryAbilities.Count;
                _activeSecondary = _secondaryAbilities[_idxSecondary];
            }
            else
            {
                _idxPrimary = (_idxPrimary - 1 + _primaryAbilities.Count) % _primaryAbilities.Count;
                _activePrimary = _primaryAbilities[_idxPrimary];
            }
        }

        public void UsePrimary()
        {
            if (_activePrimary == null || _isLocked)
                return;

            _state = AbilityState.AS_USING_PRIMARY;
            _activePrimary.Use();
        }

        public void UseSecondary()
        {
            if (_activeSecondary == null || _isLocked)
                return;

            _state = AbilityState.AS_USING_SECONDARY;
            _activeSecondary.Use();
        }

        public void ResetPrimaryTimer()
        {
            if (_activePrimary == null || _isLocked)
                return;

            _activePrimary.ResetTimer();
        }

        public void ResetSecondaryTimer()
        {
            if (_activeSecondary == null || _isLocked)
                return;

            _activeSecondary.ResetTimer();
        }

        public void Lock()
        {
            _isLocked = true;
        }
        public void Unlock()
        {
            _isLocked = false;
        }
        #endregion
    }
}
