using System.Collections.Generic;
using UnityEngine;

public class LandingPadManager : MonoBehaviour {
   private List<LandingPad> landingPads;

   public static LandingPadManager instance;
   
   private void Awake() {
      instance = this;
      landingPads = new List<LandingPad>();
      foreach (Transform child in transform) {
         LandingPad pad = child.GetComponent<LandingPad>();
         if (pad != null) {
            landingPads.Add(pad);
         }
      }
   }

   public void CheckWin()
   {
      int padsStillActive = 0;
      foreach (LandingPad pad in landingPads)
      {
         if (pad.active)
         {
            padsStillActive++;
         }
      }

      if (padsStillActive <= 1)
      {
         EndScreen.instance.BringUpScoreScreen(true);
      }
   }

   public void EnableAllPads() {
      foreach (LandingPad pad in landingPads) {
         pad.Activate();
      }
   }

   public void CheatEnablePads(float f) {
      EnableAllPads();
   }

   public Transform[] GetClosestPads(bool onlyAvailable, Transform player) {
      List<LandingPad> padsToUse = onlyAvailable ? GetAvailablePads() : landingPads;
      Transform[] padsLocation = QuicksortPadsList(padsToUse, player);

      return padsLocation;
   }

   private List<LandingPad> GetAvailablePads() {
      List<LandingPad> padsToReturn = new List<LandingPad>();
      foreach (LandingPad pad in landingPads) {
         if (pad.active) {
            padsToReturn.Add(pad);
         }
      }

      return padsToReturn;
   }

   private Transform[] QuicksortPadsList(List<LandingPad> padList, Transform player) {
      Transform[] padArray = new Transform[padList.Count];
      for (int i = 0; i < padList.Count; i++) {
         padArray[i] = padList[i].transform;
      }
      Quicksort.QuicksortByDistance(ref padArray, player);
      return padArray;
   }
}
