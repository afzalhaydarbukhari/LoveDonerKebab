
document.addEventListener('DOMContentLoaded', function () {
    const buttons = document.querySelectorAll('.menu-tab');
    const categories = document.querySelectorAll('.menu-category');
    const allProductsButton = document.querySelector('[data-category="all-products"]');

    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const category = this.getAttribute('data-category');

            // Remove active class from all buttons and add to the clicked one
            buttons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');

            if (category === 'all-products') {
                // Show all categories and items
                categories.forEach(cat => cat.style.display = 'block');
                document.querySelectorAll('.menu-item').forEach(item => {
                    item.style.display = 'block';
                });
            } else {
                // Hide all categories first
                categories.forEach(cat => cat.style.display = 'none');

                // Show only the selected category
                const selectedCategory = document.getElementById(category);
                if (selectedCategory) {
                    selectedCategory.style.display = 'block';
                }
            }
        });
    });

    // Ensure "All Products" is selected by default
    if (allProductsButton) {
        allProductsButton.click();
    }
});
