import Matcher from "./components/Matcher";
import generateRandomId from "./helper/generateRandomId"
import MatcherShell from "./components/MatcherShell.ts";
import 'bootstrap/dist/css/bootstrap.min.css';


function removeMatcherRow(id: string): void {
    console.log(id);
    const matcherTableElement = document.getElementById('matcherTableBody');
    const matcherRow = document.getElementById(id);
    if (matcherTableElement && matcherRow) {
        matcherTableElement.removeChild(matcherRow);
    }
}

function generateMatcherRow(): Matcher {
    let id = generateRandomId(10);
    return new Matcher({
        id: id,
        pattern: '/',
        ignoreCase: true,
        onClick: removeMatcherRow,
    });
}

function generateMatcherShell() : MatcherShell {

    let matchers : Matcher[] = [];
    for (let i = 0; i < 10; i++) {
        matchers.push(generateMatcherRow());
    }

    return new MatcherShell(matchers);
}

document.addEventListener('DOMContentLoaded', () => {
    const matcherTableElement = document.getElementById('app');

    let matcherShell = generateMatcherShell();
    if (matcherTableElement) {
        matcherTableElement.appendChild(matcherShell.render());
    }
});