function validateAndSend() {
    // Form field references
    const firstName = document.getElementById("contact-first-name");
    const lastName = document.getElementById("contact-last-name");
    const email = document.getElementById("contact-email");
    const message = document.getElementById("contact-message").value;
    const websiteLink = "https://lovedonerkebab.com"; // Replace with your website link
    const whatsappNumber = "+923352386386"; // Replace with your WhatsApp number

    let valid = true;

    // Validate each required field
    [firstName, lastName, email].forEach(input => {
        const error = input.nextElementSibling;
        if (input.value.trim() === "") {
            error.style.display = "block";
            valid = false;
        } else {
            error.style.display = "none";
        }
    });

    // If all fields are filled, send data to WhatsApp
    if (valid) {
        const whatsappMessage = `I visited your website: ${websiteLink}\n` +
            `First Name: ${firstName.value}\n` +
            `Last Name: ${lastName.value}\n` +
            `Email: ${email.value}\n` +
            `Message: ${message}`;

        const whatsappUrl = `https://wa.me/${whatsappNumber}?text=${encodeURIComponent(whatsappMessage)}`;
        window.open(whatsappUrl, "_blank");

        // Clear form fields after a short delay
        setTimeout(() => {
            document.getElementById("contactForm").reset();
        }, 500); // Adjust the delay if needed
    }
}