using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(StatusMvc.App_Start.MySuperPackage), "PreStart")]

namespace StatusMvc.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}