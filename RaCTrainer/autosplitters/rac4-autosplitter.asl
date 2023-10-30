// Only works on PAL

state("racman") { }

init
{
    System.IO.MemoryMappedFiles.MemoryMappedFile mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.OpenExisting("racman-autosplitter");
    System.IO.MemoryMappedFiles.MemoryMappedViewStream stream = mmf.CreateViewStream();
    vars.reader = new System.IO.BinaryReader(stream);

    // Update values from memory
    vars.UpdateValues = (Action) (() => {
        uint planet = 0;            // current planet
        uint loadPlanet = 0;        // load planet
        float voxHP = 0;            // Vox HP
        uint cutscene = 0;          // cutscene
        uint inGame = 0;            // in game
        uint tutorialFlag = 0;      // tutorial flag (0 = tutorial not completed, 1 = tutorial completed)
        uint isLoading = 0;         // loading screen (0 = not loading, 1 = loading)

        vars.reader.BaseStream.Position = 0;

        // Read values from memory
        current.planet = vars.reader.ReadUInt32();
        current.loadPlanet = vars.reader.ReadUInt32();
        current.voxHP = vars.reader.ReadSingle();
        current.cutscene = vars.reader.ReadUInt32();
        current.inGame = vars.reader.ReadUInt32();
        current.tutorialFlag = vars.reader.ReadUInt32();
        current.isLoading = vars.reader.ReadUInt32();
    });
    vars.UpdateValues();

    // Initialize run values
    vars.ResetRunValues = (Action) (() => {
        vars.splitOnCurrentPlanet = false;
    });
}

update
{
    vars.UpdateValues();
    //print(current.tutorialFlag.ToString() + " - " + current.isLoading.ToString());
    //print(old.isLoading.ToString() + " - " + current.isLoading.ToString() + " - " + current.tutorialFlag.ToString() + " - " + current.loadPlanet.ToString());
}

onReset
{
    vars.ResetRunValues();
}

split
{
    if (!settings.SplitEnabled)
    {
        return false;
    }

    if (old.planet != current.planet)
    {
        vars.splitOnCurrentPlanet = false;
    }

    // planet split
    if (old.loadPlanet != current.loadPlanet && current.loadPlanet != 15 && old.loadPlanet != 0 && current.loadPlanet != current.planet && !vars.splitOnCurrentPlanet && current.planet != 0)
    {
        print("Split on planet " + current.loadPlanet + "->" + old.loadPlanet);
        vars.splitOnCurrentPlanet = true;
        return true;
    }

    // Vox split
    if (current.planet == 15 && current.voxHP <= 0.0f && old.cutscene == 0 && current.cutscene == 1)    
    {
        print("Split on Vox");
        return true;
    }
}

reset
{
    if (!settings.ResetEnabled)
    {
        return false;
    }

    if (current.planet == 0 && old.inGame == 1 && current.inGame == 0)
    {
        return true;
    }

    if (old.isLoading == 0 && current.isLoading == 1 && current.tutorialFlag == 0 && current.loadPlanet == 1 && old.inGame == 1)
    {
        return true;
    }
}

start
{
    if (!settings.StartEnabled)
    {
        return false;
    }

    // if in main menu
    if (old.inGame == 0 && current.inGame == 1 && old.cutscene == 0 && current.tutorialFlag == 0)
    {
        return true;
    }

    // if in game
    if (old.isLoading == 0 && current.isLoading == 1 && current.tutorialFlag == 0 && current.loadPlanet == 1)
    {
        return true;
    }
}