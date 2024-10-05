import MatcherShell from "./MatcherShell.js";


export class Section {

public Name : string
public MatcherShells : MatcherShell[]

    constructor(name:string, matcherShells: MatcherShell[]) {
    this.Name = name;
    this.MatcherShells = matcherShells;
    }
    
    render() : HTMLElement
    {
        let borderDiv = document.createElement("div");
        
        let header = document.createElement("h2");
        header.textContent = this.Name;
        borderDiv.appendChild(header);
        
        return borderDiv;
    }
}

