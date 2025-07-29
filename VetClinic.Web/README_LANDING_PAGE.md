# New Landing Page for Veterinary Clinic

## Overview

Created a beautiful and modern landing page for the veterinary clinic with the following features:

### 🎨 Design

-   **Hero Section**: Gradient background with image and call-to-action buttons
-   **Features Section**: Introduction of 3 clinic strengths
-   **Services Section**: Display 6 featured services
-   **Doctors Section**: Introduction of 4 main doctors
-   **Statistics Section**: Clinic statistics
-   **Contact Section**: Contact form and contact information
-   **Call-to-Action**: Encourage registration/login

### 📱 Responsive Design

-   Compatible with all devices (desktop, tablet, mobile)
-   Smooth animations when scrolling
-   Hover effects for cards and buttons

### 🔧 Technical Features

-   **Smooth Scrolling**: Smooth scrolling to sections
-   **Intersection Observer**: Animations when elements appear
-   **Dynamic Data**: Load real data from database
-   **Error Handling**: Graceful error handling

## File Structure

### 1. Landing Page (Index.cshtml)

-   **Location**: `VetClinic.Web/Pages/Index.cshtml`
-   **Purpose**: Home page for unauthenticated users
-   **Features**:
    -   Hero section with gradient background
    -   Features cards with icons
    -   Services grid with pricing
    -   Doctors profiles
    -   Statistics counters
    -   Contact form

### 2. Dashboard Page (Dashboard.cshtml)

-   **Location**: `VetClinic.Web/Pages/Dashboard.cshtml`
-   **Purpose**: Dashboard for authenticated users
-   **Features**:
    -   Role-based dashboard (Admin, Manager, Doctor, Customer, Staff)
    -   Statistics cards
    -   Quick actions
    -   Recent activities

### 3. Code-behind Files

-   **Index.cshtml.cs**: Load data for landing page
-   **Dashboard.cshtml.cs**: Load data for dashboard

## Display Data

### Services

-   Load from `IServiceService.GetActiveServicesAsync()`
-   Display: Name, description, duration, price
-   Limit: 6 services on landing page

### Doctors

-   Load from `IUserService.GetUsersByRoleAsync("Doctor")`
-   Display: Name, email, phone number
-   Limit: 4 doctors on landing page

### Statistics

-   **Total Doctors**: Number of doctors
-   **Total Services**: Number of services
-   **Total Customers**: Number of customers
-   **Total Pets**: Number of pets

## Navigation Updates

### Layout Changes

-   **Authenticated Users**: Dashboard link → `/Dashboard`
-   **Unauthenticated Users**: Home link → `/Index`

### Login Redirect

-   Sau khi đăng nhập thành công → `/Dashboard`
-   Thay vì `/Index` như trước

## CSS Styling

### Color Scheme

-   **Primary**: Bootstrap primary blue
-   **Secondary**: Success green, danger red
-   **Gradients**: Purple to blue hero background

### Animations

-   **Fade In Up**: Hero content
-   **Fade In Right**: Hero image
-   **Hover Effects**: Cards và buttons
-   **Scroll Animations**: Intersection Observer

### Responsive Breakpoints

-   **Mobile**: < 768px
-   **Tablet**: 768px - 992px
-   **Desktop**: > 992px

## Cách Sử Dụng

### 1. Truy Cập Landing Page

```
http://localhost:5000/
```

### 2. Truy Cập Dashboard (Sau khi đăng nhập)

```
http://localhost:5000/Dashboard
```

### 3. Navigation

-   **Khách chưa đăng nhập**: Home → Landing Page
-   **Người dùng đã đăng nhập**: Dashboard → Role-based Dashboard

## Tùy Chỉnh

### Thay Đổi Số Lượng Hiển Thị

```csharp
// Trong Index.cshtml.cs
Services = services.Take(6).ToList(); // Thay đổi số 6
Doctors = doctors.Take(4).ToList();   // Thay đổi số 4
```

### Thay Đổi Màu Sắc

```css
/* Trong Index.cshtml */
.hero-section {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}
```

### Thêm Section Mới

1. Thêm HTML section trong Index.cshtml
2. Thêm CSS styling
3. Thêm JavaScript animations nếu cần

## Lưu Ý

1. **Database**: Cần có dữ liệu services và doctors trong database
2. **Images**: Hero image sử dụng Unsplash URL, có thể thay đổi
3. **Contact Form**: Hiện tại chỉ là UI, cần implement backend
4. **Statistics**: Load từ database thực tế
5. **Error Handling**: Graceful fallback khi không có dữ liệu

## Tương Lai

### Có Thể Cải Thiện

-   [ ] Implement contact form backend
-   [ ] Add image upload cho doctors
-   [ ] Add testimonials section
-   [ ] Add blog/news section
-   [ ] Add appointment booking từ landing page
-   [ ] Add multi-language support
-   [ ] Add dark mode toggle
-   [ ] Add loading animations
-   [ ] Add SEO optimization
-   [ ] Add analytics tracking
