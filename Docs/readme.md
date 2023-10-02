# Problem to solve

### ES
Generar acta de nacimiento, individuo, datos de            la persona, mama y papa, que pasa si solo es papa o mama, si solo la registra mama o papa, abuelos paternos y maternos, lugar de nacimiento, oficialia del registro civil que lo registra, fecha de asientamiento, fecha de nacimiento ( de registro), datos de oficialia


### EN

Generate birth certificate, individual, data of the person, mother and father, what happens if it is only father or mother, if it is only mother or father, paternal and maternal grandparents, place of birth, civil registry office that registers it, date of registration, date of birth (of registration), data of the civil registry office.

## How is it going to be implemented

I'm planing to implement a MVVM for the registry office workers, as they'll be able to modify and create new registers.

Same goes for the clients but they'll be limited to only making query's from the GUI depending on the data the user provides.


So i'll probably create 2 different main windows, one for the user and one for the workers, as well a special permission for admin of the DB so he can register new employees.