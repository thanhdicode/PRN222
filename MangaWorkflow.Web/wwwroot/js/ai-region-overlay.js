document.addEventListener('DOMContentLoaded', function () {
    const imgContainer = document.getElementById('image-container');
    const img = document.getElementById('manga-page-img');
    const canvas = document.getElementById('region-overlay');
    const regionItems = document.querySelectorAll('#region-list li');

    if (!imgContainer || !img || !canvas || regionItems.length === 0) {
        return;
    }

    // Function to draw bounding boxes on the canvas
    function drawRegions() {
        // Match canvas size to image size
        canvas.width = img.clientWidth;
        canvas.height = img.clientHeight;

        const ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Simulated scaling (assuming the API returns coordinates based on original image size)
        // For the mock, we just use absolute values and pretend they map to the 600x800 image
        const imgOrigWidth = 600;
        const imgOrigHeight = 800;

        const scaleX = canvas.width / imgOrigWidth;
        const scaleY = canvas.height / imgOrigHeight;

        regionItems.forEach(item => {
            const x = parseFloat(item.getAttribute('data-x'));
            const y = parseFloat(item.getAttribute('data-y'));
            const w = parseFloat(item.getAttribute('data-w'));
            const h = parseFloat(item.getAttribute('data-h'));
            
            // Extract label to choose color
            const labelText = item.querySelector('strong').innerText.toLowerCase();
            let color = 'rgba(255, 0, 0, 0.5)'; // red for character
            let borderColor = 'rgba(255, 0, 0, 1)';
            
            if (labelText.includes('text') || labelText.includes('dialogue')) {
                color = 'rgba(0, 0, 255, 0.5)'; // blue for text
                borderColor = 'rgba(0, 0, 255, 1)';
            } else if (labelText.includes('background')) {
                color = 'rgba(0, 255, 0, 0.5)'; // green for background
                borderColor = 'rgba(0, 255, 0, 1)';
            }

            const drawX = x * scaleX;
            const drawY = y * scaleY;
            const drawW = w * scaleX;
            const drawH = h * scaleY;

            // Draw border
            ctx.strokeStyle = borderColor;
            ctx.lineWidth = 2;
            ctx.strokeRect(drawX, drawY, drawW, drawH);

            // Draw fill
            ctx.fillStyle = color;
            ctx.fillRect(drawX, drawY, drawW, drawH);
            
            // Draw Label text
            ctx.fillStyle = 'white';
            ctx.font = '12px Arial';
            ctx.fillText(labelText, drawX + 2, drawY + 12);
        });
    }

    // Draw initially when image loads
    if (img.complete) {
        drawRegions();
    } else {
        img.onload = drawRegions;
    }

    // Redraw on window resize
    window.addEventListener('resize', drawRegions);

    // Interactive hovering
    regionItems.forEach(item => {
        item.addEventListener('mouseenter', function() {
            this.classList.add('bg-light');
            // We could redraw and highlight only this one, but for simplicity we keep it as is
        });
        item.addEventListener('mouseleave', function() {
            this.classList.remove('bg-light');
        });
    });
});
