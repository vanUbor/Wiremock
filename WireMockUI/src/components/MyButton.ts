import "bootstrap/dist/css/bootstrap.min.css";

interface MyButtonProps {
    label: string;
    onClick: () => void;
    variant?: string;
}

class MyButton {
    private label: string;
    private onClick: () => void;
    private variant: string;

    constructor({label, onClick, variant = "primary"}: MyButtonProps) {
        this.label = label;
        this.onClick = onClick;
        this.variant = variant;
    }

    render(): HTMLElement {
        const button = document.createElement('button');
        button.className = `btn btn-${this.variant}`;
        button.textContent = this.label;
        button.onclick = this.onClick;
        return button;
    }
}

export default MyButton;