$('#cartModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var itemName = button.data('name');
    var price = button.data('price');
    var discount = button.data('discount');
    var image = button.data('image');
    var remarks = button.data('remarks');
    var category = button.data('category');

    // Update the modal's content
    var modal = $(this);
    modal.find('#modalItemName').val(itemName); // Use .val() for input fields
    modal.find('#modalPrice').val(price);
    modal.find('#modalImage').attr('src', image);
    modal.find('#modalRemarks').val(remarks);

    // Check if discount is greater than 0 and update the modal accordingly
    if (discount > 0) {
        modal.find('#modalDiscount').val(discount + '');
        modal.find('#discountSection').show(); // Show the discount section
    } else {
        modal.find('#discountSection').hide(); // Hide the discount section
    }
});

// Category filtering logic
document.querySelectorAll('.category-btn').forEach(button => {
    button.addEventListener('click', function () {
        const selectedCategory = this.getAttribute('data-category').toLowerCase(); // Get selected category (lowercase)

        // Get all product items
        const items = document.querySelectorAll('.product-item');

        // Loop through items and show/hide based on selected category
        items.forEach(item => {
            const itemCategory = item.getAttribute('data-category').toLowerCase(); // Get item's category (lowercase)

            // Show item if category matches or if 'All' is selected
            if (selectedCategory === 'all' || itemCategory === selectedCategory) {
                item.style.display = 'block'; // Show the item
            } else {
                item.style.display = 'none'; // Hide the item
            }
        });
    });
});

document.querySelectorAll('.category-btn').forEach(button => {
    button.addEventListener('click', function () {
        const category = this.getAttribute('data-category');
    });
});