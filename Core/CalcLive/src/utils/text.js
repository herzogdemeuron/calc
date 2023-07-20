export function formatTitle(title) {
    // remove underscores and capitalize first letter of each word
    return title.replace(/_/g, ' ').replace(/\w\S*/g, (txt) => txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase());
    }

export function formatNumber(number) {
    // add decimal separator to number
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}
