using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class FireBaseController
{
    public static void TriggerEvent(int eventType, int difficulty, int levelNumber)
    {
        Firebase.Analytics.Parameter[] parameters = new Firebase.Analytics.Parameter[2];
        parameters[0] = new Firebase.Analytics.Parameter("difficulty", difficulty);
        parameters[1] = new Firebase.Analytics.Parameter("levelNumber", levelNumber);

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            switch (eventType)
            {
                case 0:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("levelReached", parameters);
                    break;
                case 1:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("TipGained");
                    break;
            }
        });
    }
}
