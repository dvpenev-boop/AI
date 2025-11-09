// This file contains JavaScript code for interactivity on the website.

document.addEventListener('DOMContentLoaded', function() {
    // Smooth scrolling for navigation links
    const links = document.querySelectorAll('a[href^="#"]');
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            targetElement.scrollIntoView({ behavior: 'smooth' });
        });
    });

    // Dynamic content loading for offers section
    const offersSection = document.getElementById('offers');
    fetch('pages/offers.html')
        .then(response => response.text())
        .then(data => {
            offersSection.innerHTML = data;
        })
        .catch(error => console.error('Error loading offers:', error));
});