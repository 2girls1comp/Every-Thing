using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using System.IO;


namespace Every_Thing
{
    public class Every_Thing : Script
    {
        bool AllPropStart = false;
        List<String> PropsKeywords = new List<String>();
        List<Prop> PropList = new List<Prop>();

        List<List<String>> matrix = new List<List<String>>(); //a list of a list of props (different lists contains different categories)
        int index = 0; //index to go through the matrix
        int indexProp = 0; //index to go through the props of each category
        int counter = 0; //count the times we spawn a new prop

        bool FuncInit = false; //bool to start the timer
        int TimerCountdown; //timer to trigger the prop
        Vector3 spawnPos;
        Random rndK = new Random();
        Random rndP = new Random();
        Random rndXYZ = new Random();
        int rangeMin = -300 * 2;
        int rangeMax = 300 * 2;

        int thingCounter = 0;
        bool showPropInfo = false;
        public Every_Thing() 
        {
            this.Tick += onTick;
            this.KeyUp += onKeyUp;
            this.KeyDown += onKeyDown;

            LoadPropsFromFiles();
        }

        private void onTick(object sender, EventArgs e)
        {
            if (AllPropStart)
            {
                int elapsedTime = (int)(Game.LastFrameTime * 1000);
                if (!FuncInit)
                {
                    TimerCountdown = 500;
                    FuncInit = true;
                }
                else
                {
                    TimerCountdown -= elapsedTime;

                    if (TimerCountdown < 0)
                    {
                        //spawn a new prop
                        //random category
                        int rndKeyword = rndK.Next(0, PropsKeywords.Count);

                        //random prop in the category
                        int rndProp = rndP.Next(0, matrix[rndKeyword].Count);

                        //random X, Y, Z from spawn position
                        float rndPropX = (float)rndXYZ.Next(rangeMin, rangeMax);
                        rndPropX *= .01f;

                        float rndPropY = (float)rndXYZ.Next(rangeMin + 600, rangeMax + 600);
                        rndPropY *= .01f;

                        float rndPropZ = (float)rndXYZ.Next(rangeMin, rangeMax);
                        rndPropZ *= .01f;

                        //UI.Notify("X = " + rndPropX + "\nY = " + rndPropY + "\nZ = " + rndPropZ);

                        Vector3 newSpawnPos = new Vector3(spawnPos.X + (rndPropX), spawnPos.Y + (rndPropY), spawnPos.Z + (rndPropZ));
                        
                        // Declare a model struct and wait for up to 250 milliseconds
                        var model = new Model(matrix[index][indexProp]);
                        model.Request(250);
                        // Check the model is valid
                        if (model.IsInCdImage && model.IsValid)
                        {
                            // Ensure the model is loaded before we try to create it in the world
                            while (!model.IsLoaded) Script.Wait(50);

                            // Create the prop in the world
                            Prop myProp = World.CreateProp(model, newSpawnPos, true, false);
                            PropList.Add(myProp);
                            if(showPropInfo)UI.Notify("Thing no." + thingCounter + ", " + matrix[index][indexProp]);
                            thingCounter++;
                        }

                        // Let the game release the model as no longer needed to remove it from memory.
                        model.MarkAsNoLongerNeeded();

                        //Prop myProp = World.CreateProp(matrix[index][indexProp], newSpawnPos, true, false);
                        //PropList.Add(myProp);


                        if (indexProp < matrix[index].Count - 1) indexProp++;
                        else
                        {
                            indexProp = 0;
                            if (index < PropsKeywords.Count - 1) index++;
                            else
                            {
                                //stop at the end of the list
                                AllPropStart = false;
                                //UI.Notify("over and out");
                                //uncomment this to loop
                                //index = 0;
                            }

                        }

                        FuncInit = false;
                        counter++;
                        if (counter % 20 == 0)
                        {
                            rangeMin -= 1;
                            rangeMax += 1;
                        }
                    }
                }
            }
        }
        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.G)
            {

                AllPropStart = true;

                if (AllPropStart)
                {
                    //save the position around which props will spawn
                    spawnPos = Game.Player.Character.Position;
                    spawnPos.Y += 3f;
                    //reset the timer
                    FuncInit = false;
                }
            }

            else if (e.KeyCode == Keys.H)
            {
                showPropInfo = !showPropInfo;
            }
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void LoadPropsFromFiles()
        {
            //get the categories
            using (var reader = new StreamReader("./scripts/props/_indexTotal.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    PropsKeywords.Add(line);
                    matrix.Add(new List<String>());
                }
            }

            //go through each category
            foreach (string keyword in PropsKeywords)
            {
                //get prop filenames of the category
                using (var reader = new StreamReader("./scripts/props/" + keyword + ".txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        matrix[index].Add(line);
                    }

                }
                //UI.Notify("Props in " + keyword + " = " + matrix[index].Count);
                if (index < PropsKeywords.Count) index++;
            }
            index = 0;
        }
    }
}
