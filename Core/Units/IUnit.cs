using Legend.Core.Actions;

namespace Legend.Core.Units {
    public interface IUnit {

        public void PerformAction(Action action) { }
        public void PerformAction(GatherAction action) { }
        public void PerformAction(BuildAction action) { }
        public void UpdateAction() { }
        public void Update() { }

    }

}
