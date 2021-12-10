using System.Linq;

namespace BingoUI.Counters
{
    public class EnemyCounter : AbstractCounter
    {
        private string enemyName;
        
        public EnemyCounter(float x, float y, string spriteName, string enemyName) : base(x, y, spriteName)
        {
            this.enemyName = enemyName;
        }
        public override string GetText() => $"{BingoUI.localSettings.Enemies.Where(pair => pair.Item2.StartsWith(enemyName)).Count()}";
        public override void Hook()
        {
            On.HealthManager.SendDeathEvent += UpdateUniqueKills;
        }

        private void UpdateUniqueKills(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            orig(self);

            if (self.gameObject.name.StartsWith(enemyName)
                && BingoUI.localSettings.Enemies.Add((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, self.gameObject.name)))
            {
                UpdateText();
            }
        }
    }
}
