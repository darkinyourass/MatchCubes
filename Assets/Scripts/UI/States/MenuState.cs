// namespace UI.States
// {
//     public class MenuState: UIBaseState
//     {
//         public MenuState(UIStateMachine currentStateContext, UIStateFactory stateFactory) : base(currentStateContext, stateFactory)
//         {
//             
//         }
//
//         public override void EnterState()
//         {
//             StateContext.MenuCanvas.gameObject.SetActive(true);
//         }
//
//         protected override void UpdateState()
//         {
//             CheckStates();
//         }
//
//         protected override void ExitState()
//         {
//             StateContext.MenuCanvas.gameObject.SetActive(false);
//         }
//
//         private void CheckStates()
//         {
//             if (StateContext.IsLevelStart)
//             {
//                 SwitchState(StateFactory.Gameplay());
//                 StateContext.IsLevelStart = false;
//             }
//         }
//     }
// }