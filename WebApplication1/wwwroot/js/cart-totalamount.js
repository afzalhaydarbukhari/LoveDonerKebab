﻿document.addEventListener("DOMContentLoaded", () => {
    console.log("Cart script loaded successfully");

    const updateCartSummary = () => {
        let totalItems = 0;
        let totalPrice = 0;

        // Iterate over all items in the cart to calculate totals
        document.querySelectorAll(".cart-inline-item").forEach((item) => {
            const qtyInput = item.querySelector(".form-input");
            const qty = parseInt(qtyInput.value) || 0;
            const price = parseFloat(qtyInput.dataset.price) || 0;

            totalItems += qty;
            totalPrice += qty * price;

            // Update the item subtotal in the UI
            const itemTotalElement = item.querySelector(".cart-inline-title span");
            if (itemTotalElement) {
                itemTotalElement.innerText = (qty * price).toFixed(2);
            }
        });

        // Update the UI for total price and total items
        document.getElementById("total-price").innerText = totalPrice.toFixed(2);
        document.getElementById("cart-total-items").innerText = totalItems;

        // Sync the hidden input fields with total values
        document.getElementById("form-total-price").value = totalPrice.toFixed(2);
        document.getElementById("form-total-items").value = totalItems;
    };

    // Attach event listeners to increment and decrement buttons
    document.querySelectorAll(".btn-increment, .btn-decrement").forEach((btn) => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();

            const cartID = btn.getAttribute("data-item-id");
            const isIncrement = btn.classList.contains("btn-increment");

            if (!cartID) {
                console.error("CartID is not defined for the button clicked.");
                return;
            }

            const qtyInput = document.getElementById(`item-qty-${cartID}`);
            let currentQty = parseInt(qtyInput.value);

            if (isIncrement) {
                currentQty++;
            } else if (currentQty > 1) {
                currentQty--;
            }

            qtyInput.value = currentQty;

            // Send updated quantity to the server
            fetch('/Home/UpdateCartQuantity', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ cartId: cartID, quantity: currentQty })
            })
                .then((response) => response.json())
                .then((result) => {
                    console.log("Server response:", result);
                    if (result.success) {
                        const itemTotal = document.getElementById(`item-total-${cartID}`);
                        itemTotal.innerText = result.updatedTotal.toFixed(2);
                        updateCartSummary();
                    } else {
                        alert(result.message || "Failed to update the cart.");
                    }
                })
                .catch((error) => {
                    console.error("Error updating cart:", error);
                });
        });
    });

    // Ensure the total price and items are updated before form submission
    document.getElementById("Clientreg").addEventListener("submit", function () {
        updateCartSummary();
    });

    // Initial cart summary update
    updateCartSummary();
});
