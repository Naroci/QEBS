using System;
using System.Collections.Generic;

namespace QEBS.Base
{
    public class AnimationEventArgs : GameEventArgs
    {
        public AnimationEventArgs()
        {
        }

        public AnimationEventArgs(bool isInstant = false)
        {
            this.SetIfIsInstantEvent(isInstant);
        }

        public AnimationEventArgs(DateTime timeStampOverride)
        {
            OverrideTimeStamp(timeStampOverride);
        }

        public AnimationEventArgs(string _AnimationPlayerName,string _AnimationName)
        {
            this.AnimationPlayerName = _AnimationPlayerName;
            this.AnimationName = _AnimationName;

         
        }


        public AnimationEventArgs(string _AnimationPlayerName,string _AnimationName,float AnimationLength_)
        {
            this.AnimationPlayerName = _AnimationPlayerName;
            this.AnimationName = _AnimationName;
            this.AnimationLength = AnimationLength_;

        }


        public AnimationEventArgs(string _AnimationPlayerName,string _AnimationName, List<Argument> _AnimationArguments = null, bool playbackwards = false)
        {
            this.AnimationPlayerName = _AnimationPlayerName;
            this.AnimationName = _AnimationName;

            if (_AnimationArguments != null)
                this.AnimationArguments = _AnimationArguments;

            this.PlayBackwards = playbackwards;
        }

        public bool PlayBackwards;

        public string AnimationPlayerName;

        public string AnimationName;

        public float AnimationLength;

        public int AnimationSpeed;

        public List<Argument> AnimationArguments;


    }
}
