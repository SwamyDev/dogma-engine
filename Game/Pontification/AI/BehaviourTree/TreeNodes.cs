using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Pontification.AI.BehaviourTree
{
    /**
     * Base class for actions, conditions, tree nodes
     */
    public abstract class Behaviour
    {
        public enum BStatus
        {
            BH_INVALID,
            BH_SUCCESS,
            BH_FAILURE,
            BH_RUNNING
        }
        protected BStatus _status;

        protected abstract BStatus Update();

        //Called after behavior has finished running.
        protected abstract void OnTerminate(BStatus status);

        public Behaviour() 
        {
            _status = BStatus.BH_INVALID;
        }

        //Helper function which makes sure to call all functions in correct order
        public BStatus Tick()
        {
            //Update the behavior and get the new status
            _status = Update();

            if (_status != BStatus.BH_RUNNING)
            {
                //If it finished running pass it on to OnTerminate
                OnTerminate(_status);
            }

            return _status;
        }
    }

    public abstract class Composite : Behaviour
    {
        protected List<Behaviour> _behaviours;

        public Composite()
        {
            _behaviours = new List<Behaviour>();
        }
    }

    /**
     * A sequence executes all child nodes until one fails or is running in which case the sequence bails out with BH_FAILURE or BH_RUNNING.
     * If all child nodes succeed the sequence returns BH_SUCCESS.
     */
    public abstract class Sequence : Composite
    {
        private int _startIdx = 0;

        protected override Behaviour.BStatus Update()
        {
            //Update all child nodes and retreive their status
            for (int i = _startIdx; i < _behaviours.Count; i++)
            {
                BStatus status = _behaviours[i].Tick();

                // If cild node is running memorize child node.
                if (status == BStatus.BH_RUNNING)
                    _startIdx = i;
                else
                    _startIdx = 0;

                //If child nodes fails or is running the sequence does the same
                if (status != BStatus.BH_SUCCESS)
                {
                    return status;
                }

                //If all childs succeeded the sequence succeeds as well
                if (i == _behaviours.Count - 1)
                {
                    return BStatus.BH_SUCCESS;
                }
            }

            //Something went wrong
            Debug.Assert(false, "Unexpected loop exit");
            return BStatus.BH_INVALID;
        }
    }

    /**
     * A selector executes all child nodes until one succeeds or is running in which case the selector bails out with BH_SUCCESS or BG_RUNNING
     * If all child nodes fail the selector returns BH_FAILURE
     */
    public abstract class Selector : Composite
    {
        protected override Behaviour.BStatus Update()
        {
            //Update all child nodes and retreive their status
            for (int i = 0; i < _behaviours.Count; i++)
            {
                BStatus status = _behaviours[i].Tick();

                //If child nodes succeeds or is running the selector does the same
                if (status != BStatus.BH_FAILURE)
                {
                    return status;
                }

                //If all childs fail the selector fails as well
                if (i == _behaviours.Count - 1)
                {
                    return BStatus.BH_FAILURE;
                }
            }

            //Something went wrong
            Debug.Assert(false, "Unexpected loop exit");
            return BStatus.BH_INVALID;
        }
    }

    /**
     * A decorator changes the state of a child behaviour according to certain contitions
     */
    public abstract class Decorator : Behaviour
    {
        protected Behaviour _child;

        public Decorator(Behaviour child)
        {
            _child = child;
        }

        protected abstract BStatus Filter(BStatus initialState);

        protected override BStatus Update()
        {
            BStatus status = _child.Tick();

            status = Filter(status);

            return status;
        }
    }
}
