using MatchFinder.Domain.Entities;

namespace MatchFinder.Domain.Constants
{
    public static class EmailConstants
    {
        public static string BodyActivationEmail(string email) =>
            @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Kích Hoạt Tài Khoản</title>
    <style>
        body, html {
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }
        .container {
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }
        h1 {
            font-size: 24px;
            text-align: center;
            color: #333;
        }
        p {
            margin-bottom: 20px;
            color: #666;
        }
        .btn {
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff !important;
            text-decoration: none;
            border-radius: 5px;
        }
        .footer {
            margin-top: 20px;
            text-align: center;
            color: #999;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Kích Hoạt Tài Khoản</h1>
        <p>Xin chào,</p>
        <p>Chào mừng bạn đến với Match Finder. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
        <p>Để trải nghiệm dịch vụ, vui lòng kích hoạt tài khoản của bạn. Nhấn vào nút dưới đây:</p>
        <p><a href=""http://localhost:5016/Home/Resetpassword?userId=2}"" class=""btn"">Kích Hoạt Tài Khoản</a></p>
        <p>Nếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
        <p>Xin cảm ơn,</p>
        <p>Đội ngũ hỗ trợ Match Finder</p>
        <div class=""footer"">
            <p>Đây là tin nhắn tự động. Vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public static string BodyResetPasswordEmail(string email) =>
            @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Đặt Lại Mật Khẩu</title>
   <style>
        body, html {
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }
        .container {
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }
        h1 {
            font-size: 24px;
            text-align: center;
            color: #333;
        }
        p {
            margin-bottom: 20px;
            color: #666;
        }
        .btn {
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff !important;
            text-decoration: none;
            border-radius: 5px;
        }
        .footer {
            margin-top: 20px;
            text-align: center;
            color: #999;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Đặt Lại Mật Khẩu</h1>
        <p>Xin chào,</p>
        <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn. Nếu bạn không thực hiện yêu cầu này, bạn có thể bỏ qua email này.</p>
        <p>Để đặt lại mật khẩu, vui lòng nhấn vào nút dưới đây:</p>
        <p><a href=""http://localhost:5016/Home/Resetpassword?userId=2}"" class=""btn"">Đặt Lại Mật Khẩu</a></p>
        <p>Nếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
        <p>Xin cảm ơn,</p>
        <p>Đội ngũ hỗ trợ Match Finder</p>
        <div class=""footer"">
            <p>Đây là tin nhắn tự động. Vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public static string BodyForgetPasswordEmail(string token, int userId) =>
    $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Quên Mật Khẩu</title>
    <style>
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        p {{
            margin-bottom: 20px;
            color: #666;
        }}
        .btn {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff !important;
            text-decoration: none;
            border-radius: 5px;
        }}
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Quên Mật Khẩu</h1>
        <p>Xin chào,</p>
        <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn. Nếu bạn không thực hiện yêu cầu này, bạn có thể bỏ qua email này.</p>
        <p>Để đặt lại mật khẩu, vui lòng nhấn vào nút dưới đây:</p>
        <p><a href=""https://matchfinder.vn/verify-token/{token}/{userId}"" class=""btn"">Đặt Lại Mật Khẩu</a></p>
        <p>Nếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
        <p>Xin cảm ơn,</p>
        <p>Đội ngũ hỗ trợ Match Finder</p>
        <div class=""footer"">
            <p>Đây là tin nhắn tự động. Vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public static string BodyInActiveFieldEmail(Field field) =>
    $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Từ chối phê duyệt sân bóng</title>
    <style>
        /* Reset styles */
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        /* Container styles */
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        /* Heading styles */
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        /* Paragraph styles */
        p {{
            margin-bottom: 20px;
            color: #666;
        }}
        /* Table styles */
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f2f2f2;
        }}
        /* Footer styles */
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Sân bóng của bạn chưa được phê duyệt</h1>
        <p>Xin chào,</p>
        <p>Chào mừng bạn đến với Match Finder. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
        <p>Cảm ơn bạn đã đăng ký sân bóng trên hệ thống Match Finder. Chúng tôi đã xem xét thông tin sân bóng của bạn, tuy nhiên, chúng tôi rất tiếc phải thông báo rằng sân bóng của bạn chưa được phê duyệt tại thời điểm này. Dưới đây là thông tin sân bóng bạn đã đăng ký:</p>

        <table>
            <tr>
                <th>Thông tin</th>
                <th>Chi tiết</th>
            </tr>
            <tr>
                <td>Tên sân</td>
                <td>{field.Name}</td>
            </tr>
            <tr>
                <td>Địa chỉ</td>
                <td>{field.Address}</td>
            </tr>
            <tr>
                <td>Tỉnh/Thành phố</td>
                <td>{field.Province}</td>
            </tr>
            <tr>
                <td>Quận/Huyện</td>
                <td>{field.District}</td>
            </tr>
            <tr>
                <td>Xã/Phường</td>
                <td>{field.Commune}</td>
            </tr>
            <tr>
                <td>Số điện thoại</td>
                <td>{field.PhoneNumber}</td>
            </tr>
            <tr>
                <td>Giờ mở cửa</td>
                <td>{TimeSpan.FromSeconds(field.OpenTime)}</td>
            </tr>
            <tr>
                <td>Giờ đóng cửa</td>
                <td>{TimeSpan.FromSeconds(field.CloseTime)}</td>
            </tr>
        </table>

        <p>Bạn có thể chỉnh sửa thông tin sân bóng và gửi lại yêu cầu phê duyệt. Nếu bạn cần hỗ trợ hoặc có bất kỳ thắc mắc nào, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
        <p>Chúng tôi rất mong được hợp tác với bạn trong tương lai.</p>
        <p>Trân trọng,</p>
        <p>Đội ngũ Match Finder</p>
        <div class=""footer"">
            <p>Đây là email tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";
        public static string BodyReinstateFieldEmail(Field field) =>
    $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo phê duyệt lại sân bóng</title>
    <style>
        /* Reset styles */
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        /* Container styles */
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        /* Heading styles */
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        /* Paragraph styles */
        p {{
            margin-bottom: 20px;
            color: #666;
        }}
        /* Table styles */
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f2f2f2;
        }}
        /* Footer styles */
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Thông báo phê duyệt lại sân bóng</h1>
        <p>Xin chào,</p>
        <p>Chào mừng bạn đến với Match Finder. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
        <p>Chúng tôi vui mừng thông báo rằng sau khi xem xét lại, sân bóng của bạn đã được phê duyệt lại trên hệ thống Match Finder. Trạng thái sân của bạn đã được chuyển sang thành ACCEPTED. Dưới đây là thông tin sân bóng :</p>

        <table>
            <tr>
                <th>Thông tin</th>
                <th>Chi tiết</th>
            </tr>
            <tr>
                <td>Tên sân</td>
                <td>{field.Name}</td>
            </tr>
            <tr>
                <td>Địa chỉ</td>
                <td>{field.Address}</td>
            </tr>
            <tr>
                <td>Tỉnh/Thành phố</td>
                <td>{field.Province}</td>
            </tr>
            <tr>
                <td>Quận/Huyện</td>
                <td>{field.District}</td>
            </tr>
            <tr>
                <td>Xã/Phường</td>
                <td>{field.Commune}</td>
            </tr>
            <tr>
                <td>Số điện thoại</td>
                <td>{field.PhoneNumber}</td>
            </tr>
            <tr>
                <td>Giờ mở cửa</td>
                <td>{TimeSpan.FromSeconds(field.OpenTime)}</td>
            </tr>
            <tr>
                <td>Giờ đóng cửa</td>
                <td>{TimeSpan.FromSeconds(field.CloseTime)}</td>
            </tr>
        </table>
        <p>Những điều cần lưu ý:</p>

        <p>1. Sân bóng của bạn sẽ xuất hiện trở lại trong các tìm kiếm trên Match Finder.</p>
        <p>2. Bạn có thể bắt đầu nhận đặt sân mới.</p>
        <p>3. Vui lòng đảm bảo tuân thủ các nguyên tắc cộng đồng của chúng tôi để tránh bị từ chối trong tương lai.</p>
        <p>Chúng tôi đánh giá cao sự hợp tác của bạn trong việc giải quyết các vấn đề trước đó và hy vọng sẽ tiếp tục có một mối quan hệ tích cực với bạn trên nền tảng của chúng tôi.</p>
        <p>Nếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ thêm, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
        <p>Trân trọng,</p>
        <p>Đội ngũ Match Finder</p>
        <div class=""footer"">
            <p>Đây là email tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public static string BodyBanFieldEmail(Field field) =>
    $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo cấm sân bóng</title>
    <style>
        /* Reset styles */
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        /* Container styles */
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        /* Heading styles */
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        /* Paragraph styles */
        p {{
            margin-bottom: 20px;
            color: #666;
        }}
        /* Table styles */
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f2f2f2;
        }}
        /* Footer styles */
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Thông báo cấm sân bóng</h1>
        <p>Xin chào,</p>
        <p>Chào mừng bạn đến với Match Finder. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
        <p>Sau khi xem xét kỹ lưỡng, chúng tôi rất tiếc phải thông báo rằng sân bóng của bạn đã bị cấm khỏi nền tảng của chúng tôi do vi phạm nghiêm trọng các nguyên tắc cộng đồng. Dưới đây là thông tin sân bóng :</p>

        <table>
            <tr>
                <th>Thông tin</th>
                <th>Chi tiết</th>
            </tr>
            <tr>
                <td>Tên sân</td>
                <td>{field.Name}</td>
            </tr>
            <tr>
                <td>Địa chỉ</td>
                <td>{field.Address}</td>
            </tr>
            <tr>
                <td>Tỉnh/Thành phố</td>
                <td>{field.Province}</td>
            </tr>
            <tr>
                <td>Quận/Huyện</td>
                <td>{field.District}</td>
            </tr>
            <tr>
                <td>Xã/Phường</td>
                <td>{field.Commune}</td>
            </tr>
            <tr>
                <td>Số điện thoại</td>
                <td>{field.PhoneNumber}</td>
            </tr>
            <tr>
                <td>Giờ mở cửa</td>
                <td>{TimeSpan.FromSeconds(field.OpenTime)}</td>
            </tr>
            <tr>
                <td>Giờ đóng cửa</td>
                <td>{TimeSpan.FromSeconds(field.CloseTime)}</td>
            </tr>
        </table>

        <p>Nếu bạn tin rằng đây là một sự nhầm lẫn hoặc có bằng chứng mới để hỗ trợ trường hợp của mình, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi trong vòng 14 ngày kể từ ngày nhận được thông báo này.</p>
        <p>Chúng tôi hy vọng bạn hiểu rằng quyết định này nhằm duy trì chất lượng và sự an toàn cho cộng đồng Match Finder.</p>
        <p>Trân trọng,</p>
        <p>Đội ngũ Match Finder</p>
        <div class=""footer"">
            <p>Đây là email tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public static string BodyActiveFieldEmail(Field field) =>
           $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Sân bóng được phê duyệt</title>
    <style>
        /* Reset styles */
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        /* Container styles */
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        /* Heading styles */
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        /* Paragraph styles */
        p {{
            margin-bottom: 20px;
            color: #666;
        }}
        /* Table styles */
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f2f2f2;
        }}
        /* Footer styles */
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Sân bóng của bạn đã được phê duyệt</h1>
        <p>Xin chào,</p>
        <p>Chào mừng bạn đến với Match Finder. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
        <p>Chúng tôi đã phê duyệt sân bóng của bạn. Dưới đây là thông tin chi tiết:</p>

        <table>
            <tr>
                <th>Thông tin</th>
                <th>Chi tiết</th>
            </tr>
            <tr>
                <td>Tên sân</td>
                <td>{field.Name}</td>
            </tr>
            <tr>
                <td>Địa chỉ</td>
                <td>{field.Address}</td>
            </tr>
            <tr>
                <td>Tỉnh/Thành phố</td>
                <td>{field.Province}</td>
            </tr>
            <tr>
                <td>Quận/Huyện</td>
                <td>{field.District}</td>
            </tr>
            <tr>
                <td>Xã/Phường</td>
                <td>{field.Commune}</td>
            </tr>
            <tr>
                <td>Số điện thoại</td>
                <td>{field.PhoneNumber}</td>
            </tr>
            <tr>
                <td>Giờ mở cửa</td>
                <td>{TimeSpan.FromSeconds(field.OpenTime)}</td>
            </tr>
            <tr>
                <td>Giờ đóng cửa</td>
                <td>{TimeSpan.FromSeconds(field.CloseTime)}</td>
            </tr>
        </table>

        <p>Bạn có thể đăng nhập vào tài khoản của mình để quản lý sân bóng và nhận đặt sân.</p>
        <p>Nếu bạn có thắc mắc hoặc cần hỗ trợ, vui lòng liên hệ với nhóm hỗ trợ của chúng tôi.</p>
        <p>Xin cảm ơn,</p>
        <p>Đội ngũ hỗ trợ Match Finder</p>
        <div class=""footer"">
            <p>Đây là email tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public static string BodyActivationEmail(string token, int userId) =>
    $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Active account</title>
    <style>
        /* Reset styles */
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        /* Container styles */
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        /* Heading styles */
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        /* Paragraph styles */
        p {{
            margin-bottom: 20px;
            color: #666;
        }}
        /* Button styles */
        .btn {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff !important;
            text-decoration: none;
            border-radius: 5px;
        }}
        /* Footer styles */
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <p>Xin chào,</p>
        <p>Chào mừng bạn đến với Match Finder. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi </p>
        <p>Để trải nghiệm dịch vụ, vui lòng kích hoạt tài khoản của bạn. Nhấp vào nút bên dưới:</p>
        <p><a href=""https://matchfinder.vn/active/{token}/{userId}"" class=""btn"">Active</a></p>
        <p>Nếu bạn có thắc mắc hoặc cần hỗ trợ, vui lòng liên hệ với nhóm hỗ trợ của chúng tôi.</p>
        <p>Xin cảm ơn,</p>
        <p>Đội ngũ hỗ trợ</p>
        <div class=""footer"">
            <p>Đây là email tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>
";

        public const string SUBJECT_BANNED_FIELD = "Match Finder - Thông Báo Cấm Sân Bóng";
        public const string SUBJECT_ACTIVATE_FIELD = "Match Finder - Sân Bóng Của Bạn Đã Được Phê Duyệt";
        public const string SUBJECT_REINSTATE_FIELD = "Match Finder - Thông Báo Khôi Phục Lại Sân Bóng";
        public const string SUBJECT_INACTIVATE_FIELD = "Match Finder - Sân Bóng Của Bạn Đã Bị Từ Chối";
        public const string SUBJECT_RESET_PASSWORD = "Match Finder - Đặt Lại Mật Khẩu";
        public const string SUBJECT_FORGET_PASSWORD = "Match Finder - Quên Mật Khẩu";
        public const string SUBJECT_ACTIVE_ACCOUNT = "Match Finder - Kích Hoạt Tài Khoản";
    }
}