import "bootstrap/dist/css/bootstrap.min.css";
import generateRandomId from "../helper/generateRandomId"

interface MatcherProps {
    id : string;
    name: string;
    pattern: string;
    ignoreCase: boolean;
    onClick: (id : string) => void;
}

class Matcher {

    private id: string;
    private name: string;
    private pattern: string;
    private ignoreCase: boolean;
    private onRemoveClick: (id:string) => void;

    constructor(props: MatcherProps) {
        this.id = props.id;
        this.name = props.name;
        this.pattern = props.pattern;
        this.ignoreCase = props.ignoreCase;
        this.onRemoveClick = props.onClick;
    };


    render(): HTMLElement {
        let row = document.createElement("tr");
        row.setAttribute("id", this.id);

        row.appendChild(this.renderMatcherCell());
        row.appendChild(this.renderPatternCell());
        row.appendChild(this.renderIgnoreCaseCell());
        row.appendChild(this.renderRemoveButton());

        return row;
    }

    private renderMatcherCell(): HTMLElement {
        let id = generateRandomId(10);
        let matcherColumn = document.createElement("td");

        let matcherLabel = document.createElement("label");
        matcherLabel.setAttribute("for", id);

        let matcherInput = document.createElement("input");
        matcherInput.setAttribute("id", id);
        matcherInput.setAttribute("placeholder", "Name");
        matcherInput.value = this.name;

        matcherColumn.appendChild(matcherLabel);
        matcherColumn.appendChild(matcherInput);

        return matcherColumn;
    }

    private renderPatternCell(): HTMLElement {
        let id = generateRandomId(10);
        let patternCell = document.createElement("td");

        let patternLabel = document.createElement("label");
        patternLabel.setAttribute("for", id);

        let patternInput = document.createElement("input");
        patternInput.setAttribute("id", id);
        patternInput.setAttribute("placeholder", "Pattern");
        patternInput.value = this.pattern;

        patternCell.appendChild(patternLabel);
        patternCell.appendChild(patternInput);

        return patternCell;
    }

    private renderIgnoreCaseCell(): HTMLElement {
        let id = generateRandomId(10);

        let ignoreCaseCell = document.createElement("td");
        let checkDiv = document.createElement("div");
        checkDiv.classList.add("form-check");
        checkDiv.classList.add("form-switch");

        let checkInput = document.createElement("input");
        checkInput.classList.add("form-check-input");
        checkInput.setAttribute("type", "checkbox");
        checkInput.setAttribute("role", "switch");
        checkInput.setAttribute("id", id);
        checkInput.checked = this.ignoreCase;

        let checkLabel = document.createElement("label");
        checkLabel.classList.add("form-check-label");
        checkLabel.setAttribute("for", id);
        checkLabel.innerText = "Ignore Case"

        checkDiv.appendChild(checkInput)
        checkDiv.appendChild(checkLabel);
        ignoreCaseCell.appendChild(checkDiv);
        return ignoreCaseCell;
    }

    private renderRemoveButton(): HTMLElement {
        let buttonCell = document.createElement("td");
        let button = document.createElement("button");
        button.innerHTML = "X";
        button.classList.add("btn");
        button.classList.add("btn-danger");
        button.addEventListener("click", () => this.onRemoveClick(this.id));
        buttonCell.appendChild(button);
        return buttonCell;
    }
}

export default Matcher;