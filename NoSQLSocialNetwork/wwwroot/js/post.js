<script>
        document.addEventListener('DOMContentLoaded', function () {
            document.querySelectorAll('[name="btnLike"]').forEach(button => {
                button.addEventListener('click', function (e) {
                    e.preventDefault();
                    const likeButton = this;
                    const imgLike = likeButton.querySelector('img');
                    const postId = this.id.split('_')[1];
                    const isLiked = imgLike.src.includes('like-blue.png');
                    imgLike.src = isLiked ? 'images/like.png' : 'images/like-blue.png';
                    fetch(`/api/Post/Like`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({ postId })
                    })
                        .then(response => response.json())
                        .then(data => {
                            const likeCountElement = likeButton.nextElementSibling;
                            likeCountElement.textContent = data.likesCount;
                        })
                        .catch(error => {
                            imglike.src = isliked ? 'images/like-blue.png' : 'images/like.png';
                        });
                });
            });
        });
    </script>
    <script>
        $(document).ready(function () {
            $(".load-more-btn").click(function () {
                $.ajax({
                    url: "/Home/LoadMore",
                    type: "GET",
                    success: function (data) {
                        $(".post-container:last").after(data);
                    }
                });
            });
        });
    </script>
    <script>
        $(document).ready(function () {
            $(".post-images-grid").on("click", ".more-images", function () {
                var $this = $(this);
                var $postImagesGrid = $this.parent();

                // Get the images from the data-attribute
                var images = $postImagesGrid.data("images").split(",");

                // Clear existing images and add all images from the data
                $postImagesGrid.empty();
                images.forEach(function (image) {
                    $postImagesGrid.append('<img src="' + image + '" class="post-img" alt="Image ' + image + '">');
                });
            });
        });
    </script>
    <script>
        // Khi nhấn vào nút "Đăng bài"
        $('#submitPostBtn').on('click', function () {
            var content = $('#post_content').val();
            var formData = new FormData();
            formData.append("Content", content);

            // Lấy hình ảnh nếu có
            var files = $('#post_images')[0].files;
            for (var i = 0; i < files.length; i++) {
                formData.append("Images", files[i]);
            }

            // Gửi yêu cầu POST đến Controller (CreatePost)
            $.ajax({
                url: '/api/Post/Create',  // Đảm bảo đường dẫn chính xác tới controller
                type: 'POST',
                data: formData,
                processData: false,  // Đảm bảo không xử lý dữ liệu
                contentType: false,  // Đảm bảo không thay đổi kiểu dữ liệu
                success: function (response) {
                    // Hiển thị thông báo thành công
                    $('#responseMessage').html('Bài viết đã được đăng thành công!').addClass('success-message show-message').show();
                    // Reset form sau khi đăng bài
                    $('#post_content').val('');
                    $('#post_images').val('');
                     location.reload();
                },
                error: function (xhr, status, error) {
                    // Hiển thị thông báo lỗi nếu có
                    $('#responseMessage').html('Có lỗi xảy ra. Vui lòng thử lại!').addClass('error-message show-message').show();
                }
            });
        });

        // Hàm để chọn ảnh
        function chooseImage() {
            $('#post_images').click();
        }
    </script>