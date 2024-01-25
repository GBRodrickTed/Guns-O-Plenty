using UnityEngine;

namespace GunsOPlenty.Utils
{
    public class HMRPlus : MonoSingleton<HMRPlus>
    {
        string message;
        string newMessage;
        float duration;
        float startTime;
        float gradOffset;
        float gradFactor;
        bool silent;
        bool effectLoop;

        public void Start()
        {

        }

        public void Update()
        {
            if (effectLoop)
            {
                float gradTime = ((Time.time - startTime) * gradFactor + gradOffset);
                newMessage = message;
                newMessage = newMessage.Replace('$', '\n');
                newMessage = newMessage.Replace("</grad>", "</color>");
                newMessage = newMessage.Replace("<grad=rainbow>", "<color=#"+ GOPUtils.ColorToHexString(GOPUtils.LerpColorArrayNorm(GOPUtils.RainbowGrad, gradTime)) + ">");
                newMessage = newMessage.Replace("<grad=fire>", "<color=#" + GOPUtils.ColorToHexString(GOPUtils.LerpColorArrayNorm(GOPUtils.FireGrad, gradTime)) + ">");
                MonoSingleton<HudMessageReceiver>.Instance.text.text = newMessage;
                //MonoSingleton<HudMessageReceiver>.Instance.text.text = "<color=#" +  + ">testop</color>\n";
            }
        }

        public void Done()
        {
            MonoSingleton<HudMessageReceiver>.Instance.text.enabled = false;
            MonoSingleton<HudMessageReceiver>.Instance.img.enabled = false;
            effectLoop = false;
        }

        public void SendHudMessageEffect(string message, float delay, float duration = 5, bool silent = true, float gradFactor = 1f, float gradOffset = 0f)
        {
            this.message = message;
            this.duration = duration;
            this.silent = silent;
            this.gradFactor = gradFactor;
            this.gradOffset = gradOffset;
            Invoke("ShowHudMessageEffect", (float)delay);
        }

        public void SendHudMessage(string message, float delay, float duration = 5, bool silent = true)
        {
            this.message = message;
            this.duration = duration;
            this.silent = silent;
            Invoke("ShowHudMessage", (float)delay);
        }

        private void ShowHudMessage()
        {
            //MonoSingleton<HudMessageReceiver>.Instance.text.text = message;
            MonoSingleton<HudMessageReceiver>.Instance.text.text = message;
            MonoSingleton<HudMessageReceiver>.Instance.text.text = MonoSingleton<HudMessageReceiver>.Instance.text.text.Replace('$', '\n');
            MonoSingleton<HudMessageReceiver>.Instance.text.enabled = true;
            MonoSingleton<HudMessageReceiver>.Instance.img.enabled = true;
            MonoSingleton<HudMessageReceiver>.Instance.hoe.Force();
            if (!this.silent)
            {
                MonoSingleton<HudMessageReceiver>.Instance.aud.Play();
            }
            MonoSingleton<HudMessageReceiver>.Instance.CancelInvoke("Done");
            effectLoop = false;
            base.CancelInvoke("Done");
            base.Invoke("Done", duration);
        }

        private void ShowHudMessageEffect()
        {
            MonoSingleton<HudMessageReceiver>.Instance.text.text = "";
            MonoSingleton<HudMessageReceiver>.Instance.text.enabled = true;
            MonoSingleton<HudMessageReceiver>.Instance.img.enabled = true;
            MonoSingleton<HudMessageReceiver>.Instance.hoe.Force();
            if (!this.silent)
            {
                MonoSingleton<HudMessageReceiver>.Instance.aud.Play();
            }
            MonoSingleton<HudMessageReceiver>.Instance.CancelInvoke("Done");
            effectLoop = true;
            startTime = Time.time;
            base.CancelInvoke("Done");
            base.Invoke("Done", duration);
        }
    }
}
