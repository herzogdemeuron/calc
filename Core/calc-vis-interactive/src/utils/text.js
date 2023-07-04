export function formatTitle(title) {
    // remove underscores and capitalize first letter of each word
    return title.replace(/_/g, ' ').replace(/\w\S*/g, (txt) => txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase());
    }