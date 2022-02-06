# SE-Commons
A base library for Space Engineers Prgrammabel Block scripts, eliminating boilerplate Code (primarliy in distinguishing callback sources)

## General info
As mentioned above, the main objective of this little library/framwork is to eliminate boilerplate code.
When writing PB scripts for SE, two tasks that come up all the time are
- responding to call to ```Main()``` and figuring out where the call came from
- parsing the blocks custom config section into an ini data structure

To remove those tasks from the programmers responsibility, the genral architecture of scripts had to be modified slightly:
- the ```Program()``` constructor, ```Main()``` method and ```Save()``` method are no longer written by the programmer, instead they are provided by this library
- all code now lives inside the ```Implementation``` class rather than Program

The ScriptBase class provides basic implementations of the following callback methods:
- Main ( string argument, UpdateType updateSource )
- Save()
- OnTrigger ( MyGridProgram parent, string arguments )
- OnScript ( MyGridProgram parent, string arguments )
- OnTerminal ( MyGridProgram parent, string arguments )
- OnMod ( MyGridProgram parent, string arguments )
- OnIGC ( MyGridProgram parent, string callback )
- OnCallOnce ( MyGridProgram parent )
- OnUpdate1 ( MyGridProgram parent )
- OnUpdate10 ( MyGridProgram parent )
- OnUpdate100 ( MyGridProgram parent )

The partial implementation of ```Implementation``` inherits these methods. 

By default, each of those methods throws a "Not implemented" exception. It is the Programmers responsibility to override them with an implementation.
The ```MyGridProgram parent``` parameter contains a reference to the base ```GridProgram``` class, all members of that (like ```Me``` for instance) are available through it.

**Note:** by default, ```Main()``` and ```Save()``` are provided by by the library and handle all necessary steps. Should you wish to override them anyways,
make sure to set the ```Implementation.mainOverridden``` or ```Implementation.saveOverridden``` fields to true, otherwise they will not be called by the parent ```Program``` class.

## Usage
Below is a basic example of how to work with this Library:

```csharp
namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public partial class Implementation : ScriptBase
        {
            public Implementation(MyGridProgram parent, MyIni custom, MyIni storage, MyIniParseResult customSectionResult) : base(custom, storage, customSectionResult)
            {
                mainOverridden = false; //added for clarification, can be removed
                saveOverridden = false; //added for clarification, can be removed
                parent.Runtime.UpdateFrequency = UpdateFrequency.None; //added for clarification, can be removed
            }
            
            public override void OnTerminal(MyGridProgram parent, string arguments)
            {
                //do stuff
            }
        }
    }
}
```

In this example, the Implementation class constructor does not register a wish to be called at regular intervals,
and instead an OnTerminal method is provided wich gets called by the ScriptCallManager whenever the script is run via the PB's "Run" button.

## Architecture
This library consists of three primary components:

*Program* provides a basic implementation of the Program() constructor and Save() and Main() methods. Its main() method calls
```ScriptCallmanager.DisAssembleScriptCallInfo(string argument, UpdateType updateType, MyGridProgram parent)``` everytime it is called,
and passes the argument string to that.

*ScriptCallManager* calls the appropriate Methods of Implementation via the ScriptBase base class and passes them the relevant arguments.

*ScriptBase* provides method stubs for all callbacks and allows the ScriptCallmanager to call these methods even though they are not necessarily implemented in Implementation

*Implementation* is responsible for overriding the relevant Method stubs from ScriptBase and generally is where all application code lives (similar to Program in a normal PB script)

