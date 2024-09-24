import MyButton from "./components/MyButton";

const handleClick = () => {
    alert('Button clicked!');
};

const myButton = new MyButton({
    label: 'Click Me',
    onClick: handleClick,
    variant: 'success'
});

document.addEventListener('DOMContentLoaded', () => {
    const app = document.getElementById('app');
    if (app) {
        app.appendChild(myButton.render());
    }
});