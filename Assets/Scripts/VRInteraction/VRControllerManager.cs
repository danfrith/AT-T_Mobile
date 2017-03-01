using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRControllerManager : MonoBehaviour {

    public List<VRGripper> Controllers;
	public void AddController()
    {

    }

    public void Removecontroller()
    {

        //VRGripper
        int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);

        SteamVR_Controller.Device rightDevice = SteamVR_Controller.Input(rightIndex);
        SteamVR_Controller.Device leftDevice = SteamVR_Controller.Input(leftIndex);

    }

    private void TriggerSqueezed()
    {

    }

    public void Update()
    {
        //foreach (VRGripper controller in Controllers)
        //{
            
        //    if ((int)controller.GetController().index == -1)
        //        continue;

        //    try
        //    {
        //        var device = SteamVR_Controller.Input((int)controller.index); // Get the device 

        //        if (device.GetPressUp(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        //        {
        //            TriggerSqueezed();
        //        }

        //        if (device.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        //        {
        //            Debug.Log("TutorialStage: Trigger pressed " + controller.index.ToString());
        //            if (MainController == -1)
        //                MainController = (int)controller.index;

        //            if ((int)controller.index == MainController)
        //            {
        //                Continue(controller);

        //            }
        //            else
        //            {
        //                Back(controller);
        //            }
        //        }
        //    }
    }
}
