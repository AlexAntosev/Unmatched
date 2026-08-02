export function position(trigger, panel) {
    if (!trigger || !panel) {
        return;
    }

    const triggerRect = trigger.getBoundingClientRect();
    panel.style.minWidth = triggerRect.width + 'px';

    const panelRect = panel.getBoundingClientRect();
    let top = triggerRect.bottom + 6;
    let left = triggerRect.left;

    // flip above the trigger when there isn't room below but there is above
    if (top + panelRect.height > window.innerHeight && triggerRect.top - panelRect.height - 6 > 0) {
        top = triggerRect.top - panelRect.height - 6;
    }

    // clamp so the panel never runs off the right edge of the viewport
    const maxLeft = window.innerWidth - panelRect.width - 8;
    if (left > maxLeft) {
        left = Math.max(8, maxLeft);
    }

    panel.style.top = top + 'px';
    panel.style.left = left + 'px';
    panel.style.visibility = 'visible';
}
