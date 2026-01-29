You'll need to use the main CharacterGeneration base as the main project, adding the CharacterCreatationTest to the project as a test project so it can see what it needs.
Tests run in CharacterCreatationTest validate the running code in CharacterGeneration

Currently the project will create characters and append them to a file at CharacterGeneration\bin\Debug\net8.0\characters.json

You can add or remove character json from that file to see the results.