﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rabbit.Properties;
using Rabbit.Kernel.TurtleGraphics;
using Rabbit.Kernel.LSystems;

namespace Rabbit.GH.LSystems
{
    public class Component_LSystem:Component_LSBase
    {

        private static LSystemParser lsystemParser = LSystemParser.Instance;

      /**
         * Constructor
         */
        public Component_LSystem()
            : base("LSystem", "LSystem", "LSystem, based on a specified axiom and a set of production rules.")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager inputManager)
        {

            inputManager.Register_StringParam("Axiom", "A", "The Axiom is the first Word in the LSystem. It is also called 'seed' or 'initiator'.", "", GH_ParamAccess.item);//name, nick, description, default, isList
            inputManager.Register_GenericParam("Production Rules", "PR", "List of Production Rules used by the LSystem to generate the Words in the LSystem language.", GH_ParamAccess.list);
            inputManager.Register_IntegerParam("Number of generations", "n", "Number of words to be generated by the LSystem", 2, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Word", "W", "The last word derived by the LSystem");//name, nick, description
            pManager.Register_GenericParam("List of Words", "LW", "List of words generated by the L-System. The list contains all words, starting by the axiom, ending with the last generated word W.");//name, nick, description
            pManager.Register_GenericParam("LSystem", "LS", "The LSystem object, based on the specified axiom and production rules.");//name, nick, description
        }

        /**
         * Does the computation
         */ 
        protected override void SolveRabbitInstance(IGH_DataAccess DA)
        {
            //PARSE THE INPUT PARAMETERS:
            //ALPHABET------------------------------------------------------------------------------    
            IList<Symbol> Alphabet = new List<Symbol>();
             

            //SEED/INITIATOR/AXIOM-------------------------------------------------------------------
            String Axiom = null;
            DA.GetData<String>(0, ref Axiom);//param index, place holder

            //init the LSystem:
            DeterministicLSystem LSystem = new DeterministicLSystem(Alphabet, lsystemParser.ParseWord(Axiom));

            //Init the LSYSTEM + RULES----------------------------------------------------------------------------------
            //Parse the Rules using the LSystemParser and add it to the LSystem
            List<GH_String> ruleStrings = new List<GH_String>();
            DA.GetDataList(1, ruleStrings);
            foreach (GH_String ruleString in ruleStrings)
            {
                LSystem.AddRule(lsystemParser.ParseRule(ruleString.Value));
            }

            //GENERATE THE STRINGS-------------------------------------------------------------------
            int NumberOfStrings = 0;
            DA.GetData<int>(2, ref NumberOfStrings);//param index, place holder
            LSystem.Rewrite(NumberOfStrings);

            //OUTPUT
            //the list of words, generated by the LSystem
            //DA.SetDataList(0, LSystem.GetLanguage());            
            DA.SetData(0, LSystem.GetCurrentDerivation());
            DA.SetDataList(1, LSystem.GetLanguage());
            DA.SetData(2, LSystem);


        }

        /**
         * The Guid of the component
         */ 
        public override Guid ComponentGuid
        {
            get {
                return new Guid("{9D2583DD-6CF5-497c-8C40-B92541528337}"); 
            }
        }


        /**
         * The icon of the component
         */
        protected override Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.Custom_24x24_LSystem;
            }
        }

    }







}
