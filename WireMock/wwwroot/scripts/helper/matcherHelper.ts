import Matcher, {MatcherProps} from "../Matcher";

function toMatchers(matcherProps: MatcherProps[]): Matcher[] {

    let matchers: Matcher[] = [];
    matcherProps.forEach((matcherProp) => {
        matchers.push(new Matcher(matcherProp));
    })
    return matchers;
}

export default toMatchers;