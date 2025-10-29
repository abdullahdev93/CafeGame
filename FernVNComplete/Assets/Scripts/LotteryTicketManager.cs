using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ScratchCardAsset
{
    [ExecuteInEditMode]
    public class LotteryTicketManager : MonoBehaviour
    {
        public ScratchCard Card;
        public EraseProgress Progress;
        public SpriteRenderer TicketImage;
        public Sprite WinningImage;
        public Sprite LosingImage;

        private bool isWinningTicket;
        private bool isRevealed;
        private float prizeAmount; // Prize amount based on ticket type 

        public Button exitButton; 

        private void Start()
        {
            // Set the winning probability and prize based on the current scene
            int probability = GetWinningProbabilityForScene(out prizeAmount);

            // Randomly determine if this ticket is a winner
            isWinningTicket = UnityEngine.Random.Range(0, probability) == 0;

            // Set the ticket image based on whether it's a winner or not before scratching
            TicketImage.sprite = isWinningTicket ? WinningImage : LosingImage;

            if (Progress != null)
            {
                Progress.OnProgress += HandleScratchProgress; 
            }

            exitButton.interactable = false; 
        }

        private void OnDestroy()
        {
            if (Progress != null)
            {
                Progress.OnProgress -= HandleScratchProgress;
            }
        }

        private void HandleScratchProgress(float progress)
        {
            if (isRevealed || progress < 0.5f)
                return;

            isRevealed = true;

            exitButton.interactable = true; 

            if (isWinningTicket)
            {
                Debug.Log($"Congratulations! You won ${prizeAmount:N2}.");
                AddPrizeToEarnings(prizeAmount); // Add the prize to the player's earnings
            }
            else
            {
                Debug.Log("Sorry, this ticket is not a winner.");
            }
        }

        private void AddPrizeToEarnings(float amount)
        {
            // Update the player's total money stored in PlayerPrefs
            float currentEarnings = PlayerPrefs.GetFloat("TotalMoney", 1000f);
            currentEarnings += amount;
            PlayerPrefs.SetFloat("TotalMoney", currentEarnings);
            PlayerPrefs.Save(); 

            // Optionally trigger a money added animation in VNMenuManager
            //VNMenuManager.instance?.PlayMoneyUpValue("MoneyAdded");
        }

        public void ResetTicket()
        {
            // Set the winning probability and prize based on the current scene
            int probability = GetWinningProbabilityForScene(out prizeAmount);

            // Reset the ticket for replay or testing purposes
            isWinningTicket = UnityEngine.Random.Range(0, probability) == 0;
            isRevealed = false;

            // Update the ticket image based on the new determination
            TicketImage.sprite = isWinningTicket ? WinningImage : LosingImage;

            if (Card != null)
            {
                Card.Clear(false);
            }

            if (Progress != null)
            {
                Progress.ResetProgress();
            }

            Debug.Log("The ticket has been reset.");
        }

        private int GetWinningProbabilityForScene(out float prize)
        {
            // Get the active scene name
            string sceneName = SceneManager.GetActiveScene().name;

            // Determine the probability and prize based on the scene
            switch (sceneName)
            {
                case "LuckyBucks": // 1/100 chance
                    prize = 1000f;
                    return 100;
                case "RainbowRiches": // 1/1000 chance
                    prize = 10000f;
                    return 1000;
                case "MegaFortune": // 1/10000 chance
                    prize = 100000f;
                    return 10000;
                default: // Default probability
                    prize = 1000f;
                    return 100;
            }
        }

        public void OnExitButtonClick()
        {
            if (isWinningTicket)
            {
                if (prizeAmount >= 100000)
                    HandleHangOutClick("WinningMegaFortune");

                else if (prizeAmount >= 10000)
                    HandleHangOutClick("WinningRainbowRiches");

                else if (prizeAmount >= 1000) 
                    HandleHangOutClick("WinningLuckyBucks");
            }
            else
            {
                HandleHangOutClick("LosingLotteryTicket");
            }
        } 


        private void HandleHangOutClick(string locationFile)
        {
            // Set the starting file based on the location
            PlayerPrefs.SetString("StartingFile", locationFile);
            PlayerPrefs.Save();

            // Transfer to the VisualNovel scene
            SceneManager.LoadScene("VisualNovel");
        } 
    } 
}
