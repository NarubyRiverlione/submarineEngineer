# Dive...Dive..Dive
Emergency dive: Wall of Text incoming #

Inspired by the Starship corporation [http://www.starshipcorporation.com](Link URL) project and Prison Architect [https://www.introversion.co.uk/prisonarchitect/](Link URL) I've started developing a 2D submarine building simulator. 

## Design goals ##

* Different rooms types inside a submarine. 
* Layout and size of the rooms should be flexible, not fixed  small,medium, large rectangular as in starship corporation.
* Some rooms have to the be a specific locations. Conn needs to be below the Tower,...
* Rooms are automatically surrounds by walls (‘bulkhead’ be better word?)
* Each room should have specific requirements. Engine room needs fuel, battery needs running generator,...
* Each room should also have dedicated crew required. Sonar needs sonar man, conn needs an officer and watchstanders,...
* Submarine design is only valid if all required rooms are operational.
* Using Kanban as project planning tool (https://kanbanflow.com/board/20b5bcfae2f3d4d66f4bb019ce3a272e)
* Using Git and Bitbucket as (private) repository, see below
* Using Bitbucket Issues tracker to log bugs.
* Resources should 'magical' be available in required rooms. There needs to be a connection. Ex. The fuel tank is connected via a pipe to the engine room. Engine room is connected to the propeller via the shaft,...
* (Almost) all rooms need electricity, so cables needs to connect them to the generator/battery.
* Crew should be able to walk between there work, eating and rest location.
* Crew cannot 'fly', only walk on the lowest space of a room. Stairs are needed to change floors.
* Crew cannot walk were there is an item (pipe, cable,..)
* As rooms have walls, they also need doors to allow crew pass to another room.
* Crew has a timetable that determines where they should be (like prison architect)
* There should be 2(or more) timetables so rooms are always staffed.
* Provide different submarine outlines.
* Provide different submarine classes . Let go nucleair !  ssn / sbbn / ..


### Some unknowns: ###
* Realistic numbers for engine horepower, generator output Watts, battery, crew requirements.
* Main and trim Ballast tank and pump room needed?  Or not as this is only a 2D representation and there located in the hull.
* Calculated weight distribution(how?)
* Calculate speed.
* Rooms needs also oxygine? So needs water?
* I don't see a solution too use artwork for each room as the layout is flexible.
* I'm willing to open source the project but don't know anything about "created comments","MIT" or other license stuff. And I'm not interested to put tile in study them, I want to code not to be a lawyer.Is it repayable? Once you have a valid design what then?
* Maybe there should be a design and a running phase as startship corporation? Scenarios like “flooding”, “fire” that needs closing of doors, repair,…
* Is this a pc & Mac game or a tablet game or all?
* Switching from Unity to Xamarin & Cocossharp ?


You probably noticed the division in the lineup of goals. 
Well, the first part is done! There is a (I hope) working Alpha 1 available for pic and Mac.