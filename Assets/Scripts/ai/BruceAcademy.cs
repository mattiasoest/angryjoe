using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using static StageController;

public class BruceAcademy : Academy {

    public override void InitializeAcademy() {
        // Debug.Log("Academy INIT");
        // StageController.instance.FastStartGame();
    }

    public override void AcademyReset() {
        Debug.Log("Academy reset");
        StageController.instance.ResetArea();
    }
}