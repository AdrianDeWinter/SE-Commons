using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using Sandbox.ModAPI;
// https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts

/*
 * Documentation of Design Decisions:
 * 
 * This file contains all utilities and wrappers needed for a project,
 * such that only project logic need to be written, and no boilerplate code has to be re-written
 * 
 * While it would be optimal for ScriptBase to basically BE program,
 * inheriting MyGridProgram and providing an extendable superclass for further implementation,
 * due to SE's limitations on script structure, the workaround of splitting the Program class
 * between commons.cs and the actual implementation file hat to be taken
 * 
 * Similarly, the Implementation class is necessitated by C# not allowing for multiple inheritance
 * If it did, the preferred way would be to have Program inherit both MyGridProgram and ScriptBase,
 * and have the users Implementation class just extend Program
 * 
 * Lastly, the reason Parent is passed in each method call,
 * rather than being handled through a public instance field in the Implementation class set by Program,
 * or a private one set through the constructor,
 * is that it is desired to allow users to not specify a constructor,
 * and also not wanting to impose any restrictions on the inner structure of Implementation.
 * Instead, it should be possible to be handled by method overrides 
*/
namespace IngameScript
{
    public class ScriptBase
    {
        //provides method stubs for all the script call types
        //extending classes should override methods they intend to use with their proper implementation

        //since Main and Save need to be triggerd by the library's Program class,
        //inheriting classes must set the respective "...Overridden" flags to true,
        //if they do override these and wish for their own Main or Save methods to be called



        //If the mainOverridden flag is set,
        //the Program constructor will pass a reference to the active SCriptCallManager instance into this field,
        //in order to allow the subclass to call the ScriptCallManager itself
        public ScriptCallManager scriptCallManagerReference;

        protected readonly MyIni customIni;
        protected readonly MyIni storageIni;
        protected readonly MyIniParseResult customSectionResult;

        protected ScriptBase(MyIni custom, MyIni storage, MyIniParseResult customSectionResult)
        {
            customIni = custom;
            storageIni = storage;
            this.customSectionResult = customSectionResult;
        }
        protected ScriptBase(MyIni custom, MyIniParseResult customSectionResult)
        {
            customIni = custom;
            this.customSectionResult = customSectionResult;
        }
        protected ScriptBase(MyIni custom, MyIni storage)
        {
            customIni = custom;
            storageIni = storage;
        }
        protected ScriptBase(MyIni storage)
        {
            storageIni = storage;
        }
        protected ScriptBase(){}

        public bool mainOverridden = false;

        public virtual void Main(string argument, UpdateType updateSource)
        {
            throw new Exception("Not implemented, override in subclass or set mainOverridden false");
        }
        public bool saveOverridden = false;
        public virtual void Save()
        {
            throw new Exception("Not implemented, override in subclass or set saveOverridden false");
        }
        public virtual void OnTrigger(MyGridProgram parent, string arguments)
        {
            throw new Exception("Not implemented, should not be called by trigger");
        }

        public virtual void OnScript(MyGridProgram parent, string arguments)
        {
            throw new Exception("Not implemented, should not be called by script");
        }

        public virtual void OnTerminal(MyGridProgram parent, string arguments)
        {
            throw new Exception("Not implemented, should not be called by terminal");
        }

        public virtual void OnMod(MyGridProgram parent, string arguments)
        {
            throw new Exception("Not implemented, should not be called by mod");
        }

        public virtual void OnIGC(MyGridProgram parent, string callback)
        {
            throw new Exception("Not implemented, should not be called by igc");
        }

        public virtual void OnCallOnce(MyGridProgram parent)
        {
            throw new Exception("Not implemented, should not be called by CallOnce");
        }

        public virtual void OnUpdate1(MyGridProgram parent)
        {
            throw new Exception("Not implemented, should not be called by Update1");
        }

        public virtual void OnUpdate10(MyGridProgram parent)
        {
            throw new Exception("Not implemented, should not be called by Update10");
        }

        public virtual void OnUpdate100(MyGridProgram parent)
        {
            throw new Exception("Not implemented, should not be called by Update100");
        }
    }
    public class ScriptCallManager
    {
        private readonly ScriptBase TargetInstance;
        private readonly MyGridProgram Parent;

        public ScriptCallManager(MyGridProgram parent, ScriptBase target)
        {
            this.TargetInstance = target;
            this.Parent = parent;
        }
            
        public void DisAssembleScriptCallInfo(string argument, UpdateType updateType, MyGridProgram parent)
        {
            //disassembles the caller info and triggers the appropriate methods of "TargetInstance" via super class ScriptBase
            //if multiple flags are set, external sources(Trigger, Terminal, etc) are processed first, followed by IGC and finally
            //timers requested by the script itself, in order of frequency (Once > Update1 > Update10 > Update 100)
            
            if ((updateType & UpdateType.Trigger) != 0)
            {
                TargetInstance.OnTrigger(Parent, argument);
            }
            if ((updateType & UpdateType.Script) != 0)
            {
                TargetInstance.OnScript(Parent, argument);
            }
            if ((updateType & UpdateType.Terminal) != 0)
            {
                TargetInstance.OnTerminal(Parent, argument);
            }
            if ((updateType & UpdateType.Mod) != 0)
            {
                TargetInstance.OnMod(Parent, argument);
            }


            if ((updateType & UpdateType.IGC) != 0)
            {
                TargetInstance.OnIGC(Parent, argument);
            }


            if ((updateType & UpdateType.Once) != 0)
            {
                TargetInstance.OnCallOnce(Parent);
            }
            if ((updateType & UpdateType.Update1) != 0)
            {
                TargetInstance.OnUpdate1(Parent);
            }
            if ((updateType & UpdateType.Update10) != 0)
            {
                TargetInstance.OnUpdate10(Parent);
            }
            if ((updateType & UpdateType.Update100) != 0)
            {
                TargetInstance.OnUpdate100(Parent);
            }
        }
    }
    public partial class Implementation : ScriptBase
    {
        public static int constructorWish = 0;
        public Implementation(MyIni custom, MyIni storage, MyIniParseResult customSectionResult) : base(custom, storage, customSectionResult)
        {
            mainOverridden = false;
            saveOverridden = false;
        }
        public Implementation(MyIni custom, MyIniParseResult customSectionResult) : base(custom, customSectionResult)
        {
            mainOverridden = false;
            saveOverridden = false;
        }
        public Implementation(MyIni custom, MyIni storage) : base(custom, storage)
        {
            mainOverridden = false;
            saveOverridden = false;
        }
        public Implementation(MyIni storage) : base(storage)
        {
            mainOverridden = false;
            saveOverridden = false;
        }
        public Implementation() : base()
        {
            mainOverridden = false;
            saveOverridden = false;
        }
    }
    partial class Program : MyGridProgram
    {
        private readonly ScriptCallManager scriptCallManager;
        private readonly ScriptBase implementation;
        private readonly MyIni customIni = new MyIni();
        private readonly MyIni storageIni = new MyIni();
        private readonly MyIniParseResult customSectionResult;
        public Program()
        {
            //read custom section, store result in customSectionResult in case of user error
            //no exception is thrown here to allow Implementation to not care about the custom section
            customIni.TryParse(Me.CustomData, out customSectionResult);
            storageIni.TryParse(Storage);
            implementation = new Implementation(this, customIni, storageIni, customSectionResult);
            /*
            switch (Implementation.constructorWish)
            {
                case 4: implementation = new Implementation();break;
                case 3: implementation = new Implementation(storageIni); break;
                case 2: implementation = new Implementation(customIni, storageIni); break;
                case 1: implementation = new Implementation(customIni, customSectionResult); break;
                case 0: implementation = new Implementation(customIni, storageIni, customSectionResult);break;
                default: throw new Exception("Invalid Constructor Wish, provide an integer between 0 and 4");
            }
            */
            
            scriptCallManager = new ScriptCallManager(this, implementation);

            if (implementation.mainOverridden)
                implementation.scriptCallManagerReference = scriptCallManager;
        }
        public void Save()
        {
            //if Implementation provides its own Save(), pass the call to that
            if (implementation.saveOverridden)
                implementation.Save();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            //if Implementation provides its own Main(), pass the call to that
            if (implementation.mainOverridden)
                implementation.Main(argument, updateSource);
            //else, figure out wich methods are to be called
            else
                scriptCallManager.DisAssembleScriptCallInfo(argument, updateSource, this);
        }
    }
}
