document.addEventListener('DOMContentLoaded', function() {
    const openModalBtn = document.getElementById('openModal');
    const closeModalBtn = document.querySelector('.icon-close');
    const wrapper = document.querySelector('.wrapper');

    openModalBtn.addEventListener('click', () => {
        wrapper.style.display = 'flex';
        setTimeout(() => {
            wrapper.classList.add('active');
        }, 10);
    });

    closeModalBtn.addEventListener('click', () => {
        wrapper.classList.remove('active');
        setTimeout(() => {
            wrapper.style.display = 'none';
        }, 300); // Wait for the transition to finish
    });

    document.getElementById('goalForm').addEventListener('submit', function(e) {
        e.preventDefault();
        // Implement goal addition logic here
        console.log('Goal added!');
        wrapper.classList.remove('active');
        setTimeout(() => {
            wrapper.style.display = 'none';
        }, 300);
    });
});
