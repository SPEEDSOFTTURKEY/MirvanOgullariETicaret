/* global fbq , cookieconsent */
/* global Journal */
window['Journal'] = {
            "isPopup": false,
            "isPhone": false,
            "isTablet": false,
            "isDesktop": true,
            "filterScrollTop": false,
            "filterUrlValuesSeparator": ",",
            "countdownDay": "Day",
            "countdownHour": "Hour",
            "countdownMin": "Min",
            "countdownSec": "Sec",
            "globalPageColumnLeftTabletStatus": false,
            "globalPageColumnRightTabletStatus": false,
            "scrollTop": false,
            "scrollToTop": false,
            "notificationHideAfter": "2000",
            "quickviewPageStyleCloudZoomStatus": true,
            "quickviewPageStyleAdditionalImagesCarousel": true,
            "quickviewPageStyleAdditionalImagesCarouselStyleSpeed": "400",
            "quickviewPageStyleAdditionalImagesCarouselStyleAutoPlay": true,
            "quickviewPageStyleAdditionalImagesCarouselStylePauseOnHover": true,
            "quickviewPageStyleAdditionalImagesCarouselStyleDelay": "3000",
            "quickviewPageStyleAdditionalImagesCarouselStyleLoop": false,
            "quickviewPageStyleAdditionalImagesHeightAdjustment": "5",
            "quickviewPageStyleProductStockUpdate": false,
            "quickviewPageStylePriceUpdate": false,
            "quickviewPageStyleOptionsSelect": "none",
            "quickviewText": "Quickview",
            "mobileHeaderOn": "tablet",
            "subcategoriesCarouselStyleSpeed": "400",
            "subcategoriesCarouselStyleAutoPlay": true,
            "subcategoriesCarouselStylePauseOnHover": true,
            "subcategoriesCarouselStyleDelay": "3000",
            "subcategoriesCarouselStyleLoop": false,
            "productPageStyleImageCarouselStyleSpeed": "400",
            "productPageStyleImageCarouselStyleAutoPlay": false,
            "productPageStyleImageCarouselStylePauseOnHover": true,
            "productPageStyleImageCarouselStyleDelay": "3000",
            "productPageStyleImageCarouselStyleLoop": false,
            "productPageStyleCloudZoomStatus": true,
            "productPageStyleCloudZoomPosition": "inner",
            "productPageStyleAdditionalImagesCarousel": true,
            "productPageStyleAdditionalImagesCarouselStyleSpeed": "500",
            "productPageStyleAdditionalImagesCarouselStyleAutoPlay": true,
            "productPageStyleAdditionalImagesCarouselStylePauseOnHover": true,
            "productPageStyleAdditionalImagesCarouselStyleDelay": "3000",
            "productPageStyleAdditionalImagesCarouselStyleLoop": false,
            "productPageStyleAdditionalImagesHeightAdjustment": "",
            "productPageStyleProductStockUpdate": true,
            "productPageStylePriceUpdate": true,
            "productPageStyleOptionsSelect": "all",
            "infiniteScrollStatus": true,
            "infiniteScrollOffset": "4",
            "infiniteScrollLoadPrev": "\u00d6nceki \u00dcr\u00fcnleri Y\u00fckle",
            "infiniteScrollLoadNext": "Sonraki \u00dcr\u00fcnleri Y\u00fckle",
            "infiniteScrollLoading": "Y\u00fckleniyor...",
            "infiniteScrollNoneLeft": "Listenin sonuna ula\u015ft\u0131n\u0131z. Yeni \u00fcr\u00fcnlerimiz eklenecektir l\u00fctfen takipte kal\u0131n.",
            "checkoutUrl": "/Checkout",
            "headerHeight": "90",
            "headerCompactHeight": "50",
            "mobileMenuOn": "",
            "searchStyleSearchAutoSuggestStatus": true,
            "searchStyleSearchAutoSuggestDescription": true,
            "searchStyleSearchAutoSuggestSubCategories": true,
            "headerMiniSearchDisplay": "default",
            "stickyStatus": true,
            "stickyFullHomePadding": false,
            "stickyFullwidth": true,
            "stickyAt": "",
            "stickyHeight": "",
            "headerTopBarHeight": "25",
            "topBarStatus": true,
            "headerType": "classic",
            "headerMobileHeight": "",
            "headerMobileStickyStatus": true,
            "headerMobileTopBarVisibility": true,
            "headerMobileTopBarHeight": "30",
            "notification": [{
                "m": 137,
                "c": "65652e2a"
            }],
            "headerNotice": [{
                "m": 56,
                "c": "9e4f882c"
            }],
            "columnsCount": 0
        };

        // forEach polyfill
        if (window.NodeList && !NodeList.prototype.forEach) {
            NodeList.prototype.forEach = Array.prototype.forEach;
        }

        (function() {
            if (Journal['isPhone']) {
                return;
            }

            var wrappers = ['search', 'cart', 'cart-content', 'logo', 'language', 'currency'];
            var documentClassList = document.documentElement.classList;

            function extractClassList() {
                return ['desktop', 'tablet', 'phone', 'desktop-header-active', 'mobile-header-active', 'mobile-menu-active'].filter(function(cls) {
                    return documentClassList.contains(cls);
                });
            }

            function mqr(mqls, listener) {
                Object.keys(mqls).forEach(function(k) {
                    mqls[k].addListener(listener);
                });

                listener();
            }

            function mobileMenu() {
                console.warn('mobile menu!');

                var element = document.querySelector('#main-menu');
                var wrapper = document.querySelector('.mobile-main-menu-wrapper');

                if (element && wrapper) {
                    wrapper.appendChild(element);
                }

                var main_menu = document.querySelector('.main-menu');

                if (main_menu) {
                    main_menu.classList.add('accordion-menu');
                }

                document.querySelectorAll('.main-menu .dropdown-toggle').forEach(function(element) {
                    element.classList.remove('dropdown-toggle');
                    element.classList.add('collapse-toggle');
                    element.removeAttribute('data-toggle');
                });

                document.querySelectorAll('.main-menu .dropdown-menu').forEach(function(element) {
                    element.classList.remove('dropdown-menu');
                    element.classList.remove('j-dropdown');
                    element.classList.add('collapse');
                });
            }

            function desktopMenu() {
                console.warn('desktop menu!');

                var element = document.querySelector('#main-menu');
                var wrapper = document.querySelector('.desktop-main-menu-wrapper');

                if (element && wrapper) {
                    wrapper.insertBefore(element, document.querySelector('#main-menu-2'));
                }

                var main_menu = document.querySelector('.main-menu');

                if (main_menu) {
                    main_menu.classList.remove('accordion-menu');
                }

                document.querySelectorAll('.main-menu .collapse-toggle').forEach(function(element) {
                    element.classList.add('dropdown-toggle');
                    element.classList.remove('collapse-toggle');
                    element.setAttribute('data-toggle', 'dropdown');
                });

                document.querySelectorAll('.main-menu .collapse').forEach(function(element) {
                    element.classList.add('dropdown-menu');
                    element.classList.add('j-dropdown');
                    element.classList.remove('collapse');
                });

                document.body.classList.remove('mobile-wrapper-open');
            }

            function mobileHeader() {
                console.warn('mobile header!');

                Object.keys(wrappers).forEach(function(k) {
                    var element = document.querySelector('#' + wrappers[k]);
                    var wrapper = document.querySelector('.mobile-' + wrappers[k] + '-wrapper');

                    if (element && wrapper) {
                        wrapper.appendChild(element);
                    }

                    if (wrappers[k] === 'cart-content') {
                        if (element) {
                            element.classList.remove('j-dropdown');
                            element.classList.remove('dropdown-menu');
                        }
                    }
                });

                var search = document.querySelector('#search');
                var cart = document.querySelector('#cart');

                if (search && (Journal['searchStyle'] === 'full')) {
                    search.classList.remove('full-search');
                    search.classList.add('mini-search');
                }

                if (cart && (Journal['cartStyle'] === 'full')) {
                    cart.classList.remove('full-cart');
                    cart.classList.add('mini-cart')
                }
            }

            function desktopHeader() {
                console.warn('desktop header!');

                Object.keys(wrappers).forEach(function(k) {
                    var element = document.querySelector('#' + wrappers[k]);
                    var wrapper = document.querySelector('.desktop-' + wrappers[k] + '-wrapper');

                    if (wrappers[k] === 'cart-content') {
                        if (element) {
                            element.classList.add('j-dropdown');
                            element.classList.add('dropdown-menu');
                            document.querySelector('#cart').appendChild(element);
                        }
                    } else {
                        if (element && wrapper) {
                            wrapper.appendChild(element);
                        }
                    }
                });

                var search = document.querySelector('#search');
                var cart = document.querySelector('#cart');

                if (search && (Journal['searchStyle'] === 'full')) {
                    search.classList.remove('mini-search');
                    search.classList.add('full-search');
                }

                if (cart && (Journal['cartStyle'] === 'full')) {
                    cart.classList.remove('mini-cart');
                    cart.classList.add('full-cart');
                }

                documentClassList.remove('mobile-cart-content-container-open');
                documentClassList.remove('mobile-main-menu-container-open');
                documentClassList.remove('mobile-overlay');
            }

            function moveElements(classList) {
                if (classList.includes('mobile-header-active')) {
                    mobileHeader();
                    mobileMenu();
                } else if (classList.includes('mobile-menu-active')) {
                    desktopHeader();
                    mobileMenu();
                } else {
                    desktopHeader();
                    desktopMenu();
                }
            }

            var mqls = {
                phone: window.matchMedia('(max-width: 768px)'),
                tablet: window.matchMedia('(max-width: 1024px)'),
                menu: window.matchMedia('(max-width: ' + Journal['mobileMenuOn'] + 'px)')
            };

            mqr(mqls, function() {
                var oldClassList = extractClassList();

                if (Journal['isDesktop']) {
                    if (mqls.phone.matches) {
                        documentClassList.remove('desktop');
                        documentClassList.remove('tablet');
                        documentClassList.add('mobile');
                        documentClassList.add('phone');
                    } else if (mqls.tablet.matches) {
                        documentClassList.remove('desktop');
                        documentClassList.remove('phone');
                        documentClassList.add('mobile');
                        documentClassList.add('tablet');
                    } else {
                        documentClassList.remove('mobile');
                        documentClassList.remove('phone');
                        documentClassList.remove('tablet');
                        documentClassList.add('desktop');
                    }

                    if (documentClassList.contains('phone') || (documentClassList.contains('tablet') && Journal['mobileHeaderOn'] === 'tablet')) {
                        documentClassList.remove('desktop-header-active');
                        documentClassList.add('mobile-header-active');
                    } else {
                        documentClassList.remove('mobile-header-active');
                        documentClassList.add('desktop-header-active');
                    }
                }

                if (documentClassList.contains('desktop-header-active') && mqls.menu.matches) {
                    documentClassList.add('mobile-menu-active');
                } else {
                    documentClassList.remove('mobile-menu-active');
                }

                var newClassList = extractClassList();

                if (oldClassList.join(' ') !== newClassList.join(' ')) {
                    if (documentClassList.contains('safari') && !documentClassList.contains('ipad') && navigator.maxTouchPoints && navigator.maxTouchPoints > 2) {
                        window.fetch('index.php?route=journal3/journal3/device_detect', {
                            method: 'POST',
                            body: 'device=ipad',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            }
                        }).then(function(data) {
                            return data.json();
                        }).then(function(data) {
                            if (data.response.reload) {
                                window.location.reload();
                            }
                        });
                    }

                    if (document.readyState === 'loading') {
                        document.addEventListener('DOMContentLoaded', function() {
                            moveElements(newClassList);
                        });
                    } else {
                        moveElements(newClassList);
                    }
                }
            });

        })();

        (function() {
            var cookies = {};
            var style = document.createElement('style');
            var documentClassList = document.documentElement.classList;

            document.head.appendChild(style);

            document.cookie.split('; ').forEach(function(c) {
                var cc = c.split('=');
                cookies[cc[0]] = cc[1];
            });

            if (Journal['popup']) {
                for (let i in Journal['popup']) {
                    if (!cookies['p-' + Journal['popup'][i]['c']]) {
                        documentClassList.add('popup-open');
                        documentClassList.add('popup-center');
                        break;
                    }
                }
            }

            if (Journal['notification']) {
                for (let i in Journal['notification']) {
                    if (cookies['n-' + Journal['notification'][i]['c']]) {
                        style.sheet.insertRule('.module-notification-' + Journal['notification'][i]['m'] + '{ display:none }');
                    }
                }
            }

            if (Journal['headerNotice']) {
                for (let i in Journal['headerNotice']) {
                    if (cookies['hn-' + Journal['headerNotice'][i]['c']]) {
                        style.sheet.insertRule('.module-header_notice-' + Journal['headerNotice'][i]['m'] + '{ display:none }');
                    }
                }
            }

            if (Journal['layoutNotice']) {
                for (let i in Journal['layoutNotice']) {
                    if (cookies['ln-' + Journal['layoutNotice'][i]['c']]) {
                        style.sheet.insertRule('.module-layout_notice-' + Journal['layoutNotice'][i]['m'] + '{ display:none }');
                    }
                }
            }
        })();
function facebook_loadScript(url, callback) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    if (script.readyState) { // only required for IE <9
        script.onreadystatechange = function () {
            if (script.readyState === "loaded" || script.readyState === "complete") {
                script.onreadystatechange = null;
                if (callback) {
                    callback();
                }
            }
        };
    } else { //Others
        if (callback) {
            script.onload = callback;
        }
    }

    script.src = url;
    document.getElementsByTagName("head")[0].appendChild(script);
}
(function () {
    var enableCookieBar = '1';
    if (enableCookieBar == '1') {
        facebook_loadScript("catalog/view/javascript/facebook_business/cookieconsent.min.js");

        // CSS dosyasını yükleme
        var css = document.createElement("link");
        css.setAttribute("rel", "stylesheet");
        css.setAttribute("type", "text/css");
        css.setAttribute(
            "href",
            "catalog/view/theme/css/facebook_business/cookieconsent.min.css");
        document.getElementsByTagName("head")[0].appendChild(css);

        window.addEventListener("load", function () {
            function setConsent() {
                fbq(
                    'consent',
                    this.hasConsented() ? 'grant' : 'revoke'
                );
            }
            window.cookieconsent.initialise({
                palette: {
                    popup: {
                        background: '#237afc'
                    },
                    button: {
                        background: '#fff',
                        text: '#237afc'
                    }
                },
                cookie: {
                    name: fbq.consentCookieName
                },
                type: 'opt-out',
                showLink: false,
                content: {
                    allow: 'Kabul Et',
                    deny: 'Reddet',
                    header: 'Sitemiz Çerez Kullanıyor',
                    message: '"Kabul Et" butonuna tıklayarak <a class="cc-link" href="/SecurityPolicy/Index" , target="_blank">gizlilik politikamıza</a> ve <a class="cc-link" href="/SecurityPolicy/Index" target="_blank">çerez politikamıza</a> onay vermiş olursunuz.'
                },
                layout: 'basic-header',
                location: true,
                revokable: true,
                onInitialise: setConsent,
                onStatusChange: setConsent,
                onRevokeChoice: setConsent
            }, function (popup) {
                // Eğer çerez onayı açık değilse, çerezleri kullanabileceğimizi biliyoruz.
                if (!popup.getStatus() && !popup.options.enabled) {
                    popup.setStatus(cookieconsent.status.dismiss);
                }
            });
        });
    }
})();
(function () {
    ! function (f, b, e, v, n, t, s) {
        if (f.fbq) return;
        n = f.fbq = function () {
            n.callMethod ?
                n.callMethod.apply(n, arguments) : n.queue.push(arguments)
        };
        if (!f._fbq) f._fbq = n;
        n.push = n;
        n.loaded = !0;
        n.version = '2.0';
        n.queue = [];
        t = b.createElement(e);
        t.async = !0;
        t.src = v;
        s = b.getElementsByTagName(e)[0];
        s.parentNode.insertBefore(t, s)
    }(window,
        document, 'script', 'https://connect.facebook.net/en_US/fbevents.js');

    var enableCookieBar = '1';
    if (enableCookieBar == '1') {
        fbq.consentCookieName = 'fb_cookieconsent_status';

        (function () {
            function getCookie(t) {
                var i = ("; " + document.cookie).split("; " + t + "=");
                if (2 == i.length) return i.pop().split(";").shift()
            }
            var consentValue = getCookie(fbq.consentCookieName);
            fbq('consent', consentValue === 'dismiss' ? 'grant' : 'revoke');
        })();
    }

})();
window.isFacebookCustomerChatInHeaderAdded = 1;
window.isFacebookCustomerChatAdded = 1;
(function ($) {
    $.fn.extend({
        rotaterator: function (options) {

            var defaults = {
                fadeSpeed: 800,
                pauseSpeed: 100,
                child: null
            };

            let opts = $.extend(defaults, options);

            return this.each(function () {
               
                var obj = $(this);
                var items = $(obj.children(), obj);
                items.each(function () {
                    $(this).hide();
                })
        
                let next = opts.child ? opts.child : $(obj).children(':first');
                $(next).fadeIn(opts.fadeSpeed, function () {
                    $(next).delay(opts.pauseSpeed).fadeOut(opts.fadeSpeed, function () {
                        let nextItem = $(this).next();
                        if (nextItem.length == 0) {
                            nextItem = $(obj).children(':first');
                        }
                        $(obj).rotaterator({
                            child: nextItem,
                            fadeSpeed: opts.fadeSpeed,
                            pauseSpeed: opts.pauseSpeed
                        });
                    })
                });
            });
        }
    });
})(jQuery);
$(function () {
    $(".featured-text .rotate-text").rotaterator({
        fadeSpeed: 150,
        pauseSpeed: 1200
    });
});
(function ($) {
    $.fn.extend({
        rotaterator: function (options) {
            var defaults = {
                fadeSpeed: 500,
                pauseSpeed: 100,
                child: null,
            };

            let opts = $.extend(defaults, options);

            return this.each(function () {
             
                var obj = $(this);
                var items = $(obj.children(), obj);
                items.each(function () {
                    $(this).hide();
                });
           
                let next = opts.child ? opts.child : $(obj).children(':first');
                $(next).fadeIn(opts.fadeSpeed, function () {
                    $(next)
                        .delay(opts.pauseSpeed)
                        .fadeOut(opts.fadeSpeed, function () {
                            var next = $(this).next();
                            if (next.length === 0) {
                                next = $(obj).children(":first");
                            }
                            $(obj).rotaterator({
                                child: next,
                                fadeSpeed: opts.fadeSpeed,
                                pauseSpeed: opts.pauseSpeed,
                            });
                        });
                });
            });
        },
    });
})(jQuery);
$(function () {
    $(".featured-text .rotate-text").rotaterator({
        fadeSpeed: 90,
        pauseSpeed: 900,
    });
});
$('#button-cart, [data-quick-buy]').on('click', function () {
    var $btn = $(this);
    $.ajax({
        url: 'index.php?route=checkout/cart/add',
        type: 'post',
        data: $(
            '#product .button-group-page input[type=\'text\'], #product .button-group-page input[type=\'hidden\'], #product .button-group-page input[type=\'radio\']:checked, #product .button-group-page input[type=\'checkbox\']:checked, #product .button-group-page select, #product .button-group-page textarea, ' +
            '#product .product-options input[type=\'text\'], #product .product-options input[type=\'hidden\'], #product .product-options input[type=\'radio\']:checked, #product .product-options input[type=\'checkbox\']:checked, #product .product-options select, #product .product-options textarea, ' +
            '#product select[name="recurring_id"]'
        ),
        dataType: 'json',
        beforeSend: function () {
            $('#button-cart').button('loading');
        },
        complete: function () {
            $('#button-cart').button('reset');
        },
        success: function (json) {
            $('.alert-dismissible, .text-danger').remove();
            $('.form-group').removeClass('has-error');

            if (json['error']) {
                if (json['error']['option']) {
                    for (let i in json['error']['option']) {
                        var element = $('#input-option' + i.replace('_', '-'));

                        if (element.parent().hasClass('input-group')) {
                            element.parent().after('<div class="text-danger">' + json['error']['option'][i] + '</div>');
                        } else {
                            element.after('<div class="text-danger">' + json['error']['option'][i] + '</div>');
                        }
                    }
                }

                if (json['error']['recurring']) {
                    $('select[name=\'recurring_id\']').after('<div class="text-danger">' + json['error']['recurring'] + '</div>');
                }

                // Highlight any found errors
                $('.text-danger').parent().addClass('has-error');

                try {
                    $('html, body').animate({
                        scrollTop: $('.form-group.has-error').offset().top - 50
                    }, 'slow');
                } catch (e) {
                    // Hata yakalanmadı, boş bırakıldı
                }
            }

            if (json['success']) {
                if ($('html').hasClass('popup-options')) {
                    parent.$(".popup-options .popup-close").trigger('click');
                }

                if (json['notification']) {
                    parent.show_notification(json['notification']);
                } else {
                    parent.$('#content').parent().before('<div class="alert alert-success alert-dismissible"><i class="fa fa-check-circle"></i> ' + json['success'] + ' <button type="button" class="close" data-dismiss="alert">&times;</button></div>');
                }

                parent.$('#cart-total').html(json['total']);
                parent.$('#cart-items,.cart-badge').html(json['items_count']);

                if (json['items_count']) {
                    parent.$('#cart-items,.cart-badge').removeClass('count-zero');
                } else {
                    parent.$('#cart-items,.cart-badge').addClass('count-zero');
                }

                if (Journal['scrollToTop']) {
                    parent.$('html, body').animate({
                        scrollTop: 0
                    }, 'slow');
                }

                parent.$('.cart-content ul').load('index.php?route=common/cart/info ul li');

                if (window.location.href.indexOf('quick_buy=true') !== -1) {
                    parent.location.href = Journal['checkoutUrl'];
                }

                if ($btn.data('quick-buy') !== undefined) {
                    window.location = Journal['checkoutUrl'];
                }

                if (parent.window['_QuickCheckout']) {
                    parent.window['_QuickCheckout'].save();
                }

                if (json['redirect']) {
                    parent.location.href = json['redirect'];
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError + '\r\n' + xhr.statusText + '\r\n' + xhr.responseText);
        }
    });
});
$('.date').datetimepicker({
    language: 'tr',
    pickTime: false
});

$('.datetime').datetimepicker({
    language: 'tr',
    pickDate: true,
    pickTime: true
});

$('.time').datetimepicker({
    language: 'tr',
    pickDate: false
});

$('button[id^=\'button-upload\']').on('click', function () {
    var node = this;
    let timer;
    $('#form-upload').remove();

    $('body').prepend('<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>');

    $('#form-upload input[name=\'file\']').trigger('click');

    if (typeof timer != 'undefined') {
        clearInterval(timer);
    }

    timer = setInterval(function () {
        if ($('#form-upload input[name=\'file\']').val() != '') {
            clearInterval(timer);

            $.ajax({
                url: 'index.php?route=tool/upload',
                type: 'post',
                dataType: 'json',
                data: new FormData($('#form-upload')[0]),
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(node).button('loading');
                },
                complete: function () {
                    $(node).button('reset');
                },
                success: function (json) {
                    $('.text-danger').remove();

                    if (json['error']) {
                        $(node).parent().find('input').after('<div class="text-danger">' + json['error'] + '</div>');
                    }

                    if (json['success']) {
                        alert(json['success']);

                        $(node).parent().find('input').val(json['code']);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
                }
            });
        }
    }, 500);
});
$(function () {
    $('#review').delegate('.pagination a', 'click', function (e) {
        e.preventDefault();

        $('#review').fadeOut('slow');

        $('#review').load(this.href);

        $('#review').fadeIn('slow');
    });

    $('#review').load('index.php?route=product/product/review&product_id=1164');

    $('#button-review').on('click', function () {
        $.ajax({
            url: 'index.php?route=product/product/write&product_id=1164',
            type: 'post',
            dataType: 'json',
            data: $("#form-review").serialize(),
            beforeSend: function () {
                $('#button-review').button('loading');
            },
            complete: function () {
                $('#button-review').button('reset');
            },
            success: function (json) {
                $('.alert-dismissible').remove();

                if (json['error']) {
                    $('#review').after('<div class="alert alert-danger alert-dismissible"><i class="fa fa-exclamation-circle"></i> ' + json['error'] + '</div>');
                }

                if (json['success']) {
                    $('#review').after('<div class="alert alert-success alert-dismissible"><i class="fa fa-check-circle"></i> ' + json['success'] + '</div>');

                    $('input[name=\'name\']').val('');
                    $('textarea[name=\'text\']').val('');
                    $('input[name=\'rating\']:checked').prop('checked', false);
                }
            }
        });
    });
});


    $('.thumbnails').magnificPopup({
        type: 'image',
        delegate: 'a',
        gallery: {
            enabled: true
        }
    });



    $('.review-links a').on('click', function () {
        var $review = $('#review');
        if ($review.length) {
            $('a[href="#' + $review.closest('.module-item').attr('id') + '"]').trigger('click');
            $('a[href="#' + $review.closest('.tab-pane').attr('id') + '"]').trigger('click');
            $('a[href="#' + $review.closest('.panel-collapse').attr('id') + '"]').trigger('click');
            $review.closest('.expand-block').find('.block-expand.btn').trigger('click');

            $([document.documentElement, document.body]).animate({
                scrollTop: $review.offset().top - 100
            }, 200);
        }
    });
$('select[name=\'recurring_id\'], input[name="quantity"]').change(function () {
    $.ajax({
        url: 'index.php?route=product/product/getRecurringDescription',
        type: 'post',
        data: $('input[name=\'product_id\'], input[name=\'quantity\'], select[name=\'recurring_id\']'),
        dataType: 'json',
        beforeSend: function () {
            $('#recurring-description').html('');
        },
        success: function (json) {
            $('.alert-dismissible, .text-danger').remove();

            if (json['success']) {
                $('#recurring-description').html(json['success']);
            }
        }
    });
});
// yeni eklenenler
// Edit account sayfasında olanlar 
$(".form-group[data-sort]")
    .detach()
    .each(function () {
        if (
            $(this).attr("data-sort") >= 0 &&
            $(this).attr("data-sort") <= $(".form-group").length
        ) {
            $(".form-group").eq($(this).attr("data-sort")).before(this);
        }

        if ($(this).attr("data-sort") > $(".form-group").length) {
            $(".form-group:last").after(this);
        }

        if ($(this).attr("data-sort") == $(".form-group").length) {
            $(".form-group:last").after(this);
        }

        if ($(this).attr("data-sort") < -$(".form-group").length) {
            $(".form-group:first").before(this);
        }
    });

$("button[id^='button-custom-field']").on("click", function () {
    var element = this;
    let timer;
    $("#form-upload").remove();

    $("body").prepend(
        '<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>'
    );

    $("#form-upload input[name='file']").trigger("click");

    if (typeof timer != "undefined") {
        clearInterval(timer);
    }

    timer = setInterval(function () {
        if ($("#form-upload input[name='file']").val() != "") {
            clearInterval(timer);

            $.ajax({
                url: "index.php?route=tool/upload",
                type: "post",
                dataType: "json",
                data: new FormData($("#form-upload")[0]),
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(element).button("loading");
                },
                complete: function () {
                    $(element).button("reset");
                },
                success: function (json) {
                    $(element).parent().find(".text-danger").remove();

                    if (json["error"]) {
                        $(element)
                            .parent()
                            .find("input")
                            .after(
                                '<div class="text-danger">' + json["error"] + "</div>"
                            );
                    }

                    if (json["success"]) {
                        alert(json["success"]);

                        $(element).parent().find("input").val(json["code"]);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(
                        thrownError +
                        "\r\n" +
                        xhr.statusText +
                        "\r\n" +
                        xhr.responseText
                    );
                },
            });
        }
    }, 500);
});

$(".date").datetimepicker({
    language: "tr",
    pickTime: false,
});

$(".datetime").datetimepicker({
    language: "tr",
    pickDate: true,
    pickTime: true,
});

$(".time").datetimepicker({
    language: "tr",
    pickDate: false,
});
// new addess sayfasında olanlar 
$(".form-group[data-sort]")
    .detach()
    .each(function () {
        if (
            $(this).attr("data-sort") >= 0 &&
            $(this).attr("data-sort") <= $(".form-group").length - 2
        ) {
            $(".form-group")
                .eq(parseInt($(this).attr("data-sort")) + 2)
                .before(this);
        }

        if ($(this).attr("data-sort") > $(".form-group").length - 2) {
            $(".form-group:last").after(this);
        }

        if ($(this).attr("data-sort") == $(".form-group").length - 2) {
            $(".form-group:last").after(this);
        }

        if ($(this).attr("data-sort") < -$(".form-group").length - 2) {
            $(".form-group:first").before(this);
        }
    });

$("button[id^='button-custom-field']").on("click", function () {
    var element = this;
    let timer;
    $("#form-upload").remove();

    $("body").prepend(
        '<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>'
    );

    $("#form-upload input[name='file']").trigger("click");

    if (typeof timer != "undefined") {
        clearInterval(timer);
    }

    timer = setInterval(function () {
        if ($("#form-upload input[name='file']").val() != "") {
            clearInterval(timer);

            $.ajax({
                url: "index.php?route=tool/upload",
                type: "post",
                dataType: "json",
                data: new FormData($("#form-upload")[0]),
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(element).button("loading");
                },
                complete: function () {
                    $(element).button("reset");
                },
                success: function (json) {
                    $(element).parent().find(".text-danger").remove();

                    if (json["error"]) {
                        $(element)
                            .parent()
                            .find("input")
                            .after(
                                '<div class="text-danger">' + json["error"] + "</div>"
                            );
                    }

                    if (json["success"]) {
                        alert(json["success"]);

                        $(element).parent().find("input").val(json["code"]);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(
                        thrownError +
                        "\r\n" +
                        xhr.statusText +
                        "\r\n" +
                        xhr.responseText
                    );
                },
            });
        }
    }, 500);
});

$(".date").datetimepicker({
    language: "tr",
    pickTime: false,
});

$(".datetime").datetimepicker({
    language: "tr",
    pickDate: true,
    pickTime: true,
});

$(".time").datetimepicker({
    language: "tr",
    pickDate: false,
});

$("select[name='country_id']").on("change", function () {
    $.ajax({
        url:
            "index.php?route=account/account/country&country_id=" +
            this.value,
        dataType: "json",
        beforeSend: function () {
            $("select[name='country_id']").prop("disabled", true);
        },
        complete: function () {
            $("select[name='country_id']").prop("disabled", false);
        },
        success: function (json) {
            if (json["postcode_required"] == "1") {
                $("input[name='postcode']")
                    .parent()
                    .parent()
                    .addClass("required");
            } else {
                $("input[name='postcode']")
                    .parent()
                    .parent()
                    .removeClass("required");
            }

            let html = '<option value=""> --- Seçiniz --- </option>';

            if (json["zone"] && json["zone"] != "") {
                for (let i = 0; i < json["zone"].length; i++) {
                    html += '<option value="' + json["zone"][i]["zone_id"] + '"';

                    if (json["zone"][i]["zone_id"] == "") {
                        html += ' selected="selected"';
                    }

                    html += ">" + json["zone"][i]["name"] + "</option>";
                }
            } else {
                html +=
                    '<option value="0" selected="selected"> --- Yok --- </option>';
            }

            $("select[name='zone_id']").html(html);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(
                thrownError +
                "\r\n" +
                xhr.statusText +
                "\r\n" +
                xhr.responseText
            );
        },
    });
});

$("select[name='country_id']").trigger("change");

// edit addess sayfasında olanlar

$('.form-group[data-sort]').detach().each(function () {
    if ($(this).attr('data-sort') >= 0 && $(this).attr('data-sort') <= $('.form-group').length - 2) {
        $('.form-group').eq(parseInt($(this).attr('data-sort')) + 2).before(this);
    }

    if ($(this).attr('data-sort') > $('.form-group').length - 2) {
        $('.form-group:last').after(this);
    }

    if ($(this).attr('data-sort') == $('.form-group').length - 2) {
        $('.form-group:last').after(this);
    }

    if ($(this).attr('data-sort') < -$('.form-group').length - 2) {
        $('.form-group:first').before(this);
    }
});

$('button[id^=\'button-custom-field\']').on('click', function () {
    var element = this;
    let timer;
    $('#form-upload').remove();

    $('body').prepend('<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>');

    $('#form-upload input[name=\'file\']').trigger('click');

    if (typeof timer != 'undefined') {
        clearInterval(timer);
    }

    timer = setInterval(function () {
        if ($('#form-upload input[name=\'file\']').val() != '') {
            clearInterval(timer);

            $.ajax({
                url: 'index.php?route=tool/upload',
                type: 'post',
                dataType: 'json',
                data: new FormData($('#form-upload')[0]),
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(element).button('loading');
                },
                complete: function () {
                    $(element).button('reset');
                },
                success: function (json) {
                    $(element).parent().find('.text-danger').remove();

                    if (json['error']) {
                        $(element).parent().find('input').after('<div class="text-danger">' + json['error'] + '</div>');
                    }

                    if (json['success']) {
                        alert(json['success']);

                        $(element).parent().find('input').val(json['code']);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
                }
            });
        }
    }, 500);
});

$('.date').datetimepicker({
    language: 'tr',
    pickTime: false
});

$('.datetime').datetimepicker({
    language: 'tr',
    pickDate: true,
    pickTime: true
});

$('.time').datetimepicker({
    language: 'tr',
    pickDate: false
});

$('select[name=\'country_id\']').on('change', function () {
    $.ajax({
        url: 'index.php?route=account/account/country&country_id=' + this.value,
        dataType: 'json',
        beforeSend: function () {
            $('select[name=\'country_id\']').prop('disabled', true);
        },
        complete: function () {
            $('select[name=\'country_id\']').prop('disabled', false);
        },
        success: function (json) {
            if (json['postcode_required'] == '1') {
                $('input[name=\'postcode\']').parent().parent().addClass('required');
            } else {
                $('input[name=\'postcode\']').parent().parent().removeClass('required');
            }

            let html = '<option value=""> --- Seçiniz --- </option>';

            if (json['zone'] && json['zone'] != '') {
                for (let i = 0; i < json['zone'].length; i++) {
                    html += '<option value="' + json['zone'][i]['zone_id'] + '"';

                    if (json['zone'][i]['zone_id'] == '') {
                        html += ' selected="selected"';
                    }

                    html += '>' + json['zone'][i]['name'] + '</option>';
                }
            } else {
                html += '<option value="0" selected="selected"> --- Yok --- </option>';
            }

            $('select[name=\'zone_id\']').html(html);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
        }
    });
});

$('select[name=\'country_id\']').trigger('change');

// AddReturnRequest sayfasında olanlar
$('.date').datetimepicker({
    language: 'tr',
    pickTime: false
});
// AffiliateInformation sayfasında olanlar

$('input[name=\'payment\']').on('change', function () {
    $('.payment').hide();

    $('#payment-' + this.value).show();
});

$('input[name=\'payment\']:checked').trigger('change');

$('.form-group[data-sort]').detach().each(function () {
    if ($(this).attr('data-sort') >= 0 && $(this).attr('data-sort') <= $('.form-group').length) {
        $('.form-group').eq($(this).attr('data-sort')).before(this);
    }

    if ($(this).attr('data-sort') > $('.form-group').length) {
        $('.form-group:last').after(this);
    }

    if ($(this).attr('data-sort') == $('.form-group').length) {
        $('.form-group:last').after(this);
    }

    if ($(this).attr('data-sort') < -$('.form-group').length) {
        $('.form-group:first').before(this);
    }
});


$('button[id^=\'button-custom-field\']').on('click', function () {
    var node = this;
    let timer;
    $('#form-upload').remove();

    $('body').prepend('<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>');

    $('#form-upload input[name=\'file\']').trigger('click');

    if (typeof timer != 'undefined') {
        clearInterval(timer);
    }

    timer = setInterval(function () {
        if ($('#form-upload input[name=\'file\']').val() != '') {
            clearInterval(timer);

            $.ajax({
                url: 'index.php?route=tool/upload',
                type: 'post',
                dataType: 'json',
                data: new FormData($('#form-upload')[0]),
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(node).button('loading');
                },
                complete: function () {
                    $(node).button('reset');
                },
                success: function (json) {
                    $(node).parent().find('.text-danger').remove();

                    if (json['error']) {
                        $(node).parent().find('input').after('<div class="text-danger">' + json['error'] + '</div>');
                    }

                    if (json['success']) {
                        alert(json['success']);

                        $(node).parent().find('input').val(json['code']);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
                }
            });
        }
    }, 500);
});


$('.date').datetimepicker({
    language: 'tr',
    pickTime: false
});

$('.datetime').datetimepicker({
    language: 'tr',
    pickDate: true,
    pickTime: true
});

$('.time').datetimepicker({
    language: 'tr',
    pickDate: false
});

// CreateAccount sayfasında olanlar

// Sort the custom fields
$("#account .form-group[data-sort]")
    .detach()
    .each(function () {
        if (
            $(this).attr("data-sort") >= 0 &&
            $(this).attr("data-sort") <= $("#account .form-group").length
        ) {
            $("#account .form-group")
                .eq($(this).attr("data-sort"))
                .before(this);
        }

        if ($(this).attr("data-sort") > $("#account .form-group").length) {
            $("#account .form-group:last").after(this);
        }

        if ($(this).attr("data-sort") == $("#account .form-group").length) {
            $("#account .form-group:last").after(this);
        }

        if ($(this).attr("data-sort") < -$("#account .form-group").length) {
            $("#account .form-group:first").before(this);
        }
    });

$("input[name='customer_group_id']").on("change", function () {
    $.ajax({
        url:
            "index.php?route=account/register/customfield&customer_group_id=" +
            this.value,
        dataType: "json",
        success: function (json) {
            $(".custom-field").hide();
            $(".custom-field").removeClass("required");

            for (let i = 0; i < json.length; i++) {
                let custom_field = json[i];

                $("#custom-field" + custom_field["custom_field_id"]).show();

                if (custom_field["required"]) {
                    $("#custom-field" + custom_field["custom_field_id"]).addClass(
                        "required"
                    );
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(
                thrownError +
                "\r\n" +
                xhr.statusText +
                "\r\n" +
                xhr.responseText
            );
        },
    });
});

$("input[name='customer_group_id']:checked").trigger("change");



$("button[id^='button-custom-field']").on("click", function () {
    var element = this;
    let timer;
    $("#form-upload").remove();

    $("body").prepend(
        '<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>'
    );

    $("#form-upload input[name='file']").trigger("click");

    if (typeof timer != "undefined") {
        clearInterval(timer);
    }

    timer = setInterval(function () {
        if ($("#form-upload input[name='file']").val() != "") {
            clearInterval(timer);

            $.ajax({
                url: "index.php?route=tool/upload",
                type: "post",
                dataType: "json",
                data: new FormData($("#form-upload")[0]),
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $(element).button("loading");
                },
                complete: function () {
                    $(element).button("reset");
                },
                success: function (json) {
                    $(element).parent().find(".text-danger").remove();

                    if (json["error"]) {
                        $(element)
                            .parent()
                            .find("input")
                            .after(
                                '<div class="text-danger">' + json["error"] + "</div>"
                            );
                    }

                    if (json["success"]) {
                        alert(json["success"]);

                        $(element).parent().find("input").val(json["code"]);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(
                        thrownError +
                        "\r\n" +
                        xhr.statusText +
                        "\r\n" +
                        xhr.responseText
                    );
                },
            });
        }
    }, 500);
});


$(".date").datetimepicker({
    language: "tr",
    pickTime: false,
});

$(".time").datetimepicker({
    language: "tr",
    pickDate: false,
});

$(".datetime").datetimepicker({
    language: "tr",
    pickDate: true,
    pickTime: true,
});

// ShoppingCart sayfasında olanlar


$("#button-coupon").on("click", function () {
    $.ajax({
        url: "index.php?route=extension/total/coupon/coupon",
        type: "post",
        data:
            "coupon=" +
            encodeURIComponent(
                $("input[name='coupon']").val()
            ),
        dataType: "json",
        beforeSend: function () {
            $("#button-coupon").button("loading");
        },
        complete: function () {
            $("#button-coupon").button("reset");
        },
        success: function (json) {
            $(".alert-dismissible").remove();

            if (json["error"]) {
                $(".container").prepend(
                    '<div class="alert alert-danger alert-dismissible"><i class="fa fa-exclamation-circle"></i> ' +
                    json["error"] +
                    '<button type="button" class="close" data-dismiss="alert">&times;</button></div>'
                );

                $("html, body").animate(
                    { scrollTop: 0 },
                    "slow"
                );
            }

            if (json["redirect"]) {
                window.location = json["redirect"];
            }
        },
        error: function (
            xhr,
            ajaxOptions,
            thrownError
        ) {
            alert(
                thrownError +
                "\r\n" +
                xhr.statusText +
                "\r\n" +
                xhr.responseText
            );
        },
    });
});


$("#button-quote").on("click", function () {
    $.ajax({
        url: "index.php?route=extension/total/shipping/quote",
        type: "post",
        data:
            "country_id=" +
            $("select[name='country_id']").val() +
            "&zone_id=" +
            $("select[name='zone_id']").val() +
            "&postcode=" +
            encodeURIComponent(
                $("input[name='postcode']").val()
            ),
        dataType: "json",
        beforeSend: function () {
            $("#button-quote").button("loading");
        },
        complete: function () {
            $("#button-quote").button("reset");
        },
        success: function (json) {
            $(
                ".alert-dismissible, .text-danger"
            ).remove();

            if (json["error"]) {
                if (json["error"]["warning"]) {
                    $(".container").prepend(
                        '<div class="alert alert-danger alert-dismissible"><i class="fa fa-exclamation-circle"></i> ' +
                        json["error"]["warning"] +
                        '<button type="button" class="close" data-dismiss="alert">&times;</button></div>'
                    );

                    $("html, body").animate(
                        { scrollTop: 0 },
                        "slow"
                    );
                }

                if (json["error"]["country"]) {
                    $("select[name='country_id']").after(
                        '<div class="text-danger">' +
                        json["error"]["country"] +
                        "</div>"
                    );
                }

                if (json["error"]["zone"]) {
                    $("select[name='zone_id']").after(
                        '<div class="text-danger">' +
                        json["error"]["zone"] +
                        "</div>"
                    );
                }

                if (json["error"]["postcode"]) {
                    $("input[name='postcode']").after(
                        '<div class="text-danger">' +
                        json["error"]["postcode"] +
                        "</div>"
                    );
                }
            }

            if (json["shipping_method"]) {
                $("#modal-shipping").remove();

                let html =
                    '<div id="modal-shipping" class="modal">';
                html += '  <div class="modal-dialog">';
                html += '    <div class="modal-content">';
                html +=
                    '      <div class="modal-header">';
                html +=
                    '        <h4 class="modal-title">Lütfen bu siparişinizde kullanmak istediğiniz kargo metodunu seçiniz.</h4>';
                html += "      </div>";
                html += '      <div class="modal-body">';

                for (let i in json["shipping_method"]) {
                    html +=
                        "<p><strong>" +
                        json["shipping_method"][i]["title"] +
                        "</strong></p>";

                    if (
                        !json["shipping_method"][i]["error"]
                    ) {
                        for (let j in json["shipping_method"][i][
                            "quote"
                        ]) {
                            html += '<div class="radio">';
                            html += "  <label>";

                            if (
                                json["shipping_method"][i][
                                "quote"
                                ][j]["code"] == ""
                            ) {
                                html +=
                                    '<input type="radio" name="shipping_method" value="' +
                                    json["shipping_method"][i][
                                    "quote"
                                    ][j]["code"] +
                                    '" checked="checked" />';
                            } else {
                                html +=
                                    '<input type="radio" name="shipping_method" value="' +
                                    json["shipping_method"][i][
                                    "quote"
                                    ][j]["code"] +
                                    '" />';
                            }

                            html +=
                                json["shipping_method"][i][
                                "quote"
                                ][j]["title"] +
                                " - " +
                                json["shipping_method"][i][
                                "quote"
                                ][j]["text"] +
                                "</label></div>";
                        }
                    } else {
                        html +=
                            '<div class="alert alert-danger alert-dismissible">' +
                            json["shipping_method"][i][
                            "error"
                            ] +
                            "</div>";
                    }
                }

                html += "      </div>";
                html +=
                    '      <div class="modal-footer">';
                html +=
                    '        <button type="button" class="btn btn-default" data-dismiss="modal">İptal</button>';

                html +=
                    '        <input type="button" value="Kargoyu Uygula" id="button-shipping" data-loading-text="Yükleniyor..." class="btn btn-primary" disabled="disabled" />';

                html += "      </div>";
                html += "    </div>";
                html += "  </div>";
                html += "</div> ";

                $("body").append(html);

                $("#modal-shipping").modal("show");

                $("input[name='shipping_method']").on(
                    "change",
                    function () {
                        $("#button-shipping").prop(
                            "disabled",
                            false
                        );
                    }
                );
            }
        },
        error: function (
            xhr,
            ajaxOptions,
            thrownError
        ) {
            alert(
                thrownError +
                "\r\n" +
                xhr.statusText +
                "\r\n" +
                xhr.responseText
            );
        },
    });
});

$(document).delegate(
    "#button-shipping",
    "click",
    function () {
        $.ajax({
            url: "index.php?route=extension/total/shipping/shipping",
            type: "post",
            data:
                "shipping_method=" +
                encodeURIComponent(
                    $(
                        "input[name='shipping_method']:checked"
                    ).val()
                ),
            dataType: "json",
            beforeSend: function () {
                $("#button-shipping").button("loading");
            },
            complete: function () {
                $("#button-shipping").button("reset");
            },
            success: function (json) {
                $(".alert-dismissible").remove();

                if (json["error"]) {
                    $(".container").prepend(
                        '<div class="alert alert-danger alert-dismissible"><i class="fa fa-exclamation-circle"></i> ' +
                        json["error"] +
                        '<button type="button" class="close" data-dismiss="alert">&times;</button></div>'
                    );

                    $("html, body").animate(
                        { scrollTop: 0 },
                        "slow"
                    );
                }

                if (json["redirect"]) {
                    window.location = json["redirect"];
                }
            },
            error: function (
                xhr,
                ajaxOptions,
                thrownError
            ) {
                alert(
                    thrownError +
                    "\r\n" +
                    xhr.statusText +
                    "\r\n" +
                    xhr.responseText
                );
            },
        });
    }
);


$("select[name='country_id']").on(
    "change",
    function () {
        $.ajax({
            url:
                "index.php?route=extension/total/shipping/country&country_id=" +
                this.value,
            dataType: "json",
            beforeSend: function () {
                $("select[name='country_id']").prop(
                    "disabled",
                    true
                );
            },
            complete: function () {
                $("select[name='country_id']").prop(
                    "disabled",
                    false
                );
            },
            success: function (json) {
                if (json["postcode_required"] == "1") {
                    $("input[name='postcode']")
                        .parent()
                        .parent()
                        .addClass("required");
                } else {
                    $("input[name='postcode']")
                        .parent()
                        .parent()
                        .removeClass("required");
                }

                let html =
                    '<option value=""> --- Seçiniz --- </option>';

                if (json["zone"] && json["zone"] != "") {
                    for (let i = 0; i < json["zone"].length; i++)
                    {
                        html +=
                            '<option value="' +
                            json["zone"][i]["zone_id"] +
                            '"';

                        if (
                            json["zone"][i]["zone_id"] == "3315"
                        ) {
                            html += ' selected="selected"';
                        }

                        html +=
                            ">" +
                            json["zone"][i]["name"] +
                            "</option>";
                    }
                } else {
                    html +=
                        '<option value="0" selected="selected"> --- Yok --- </option>';
                }

                $("select[name='zone_id']").html(html);
            },
            error: function (
                xhr,
                ajaxOptions,
                thrownError
            ) {
                alert(
                    thrownError +
                    "\r\n" +
                    xhr.statusText +
                    "\r\n" +
                    xhr.responseText
                );
            },
        });
    }
);

$("select[name='country_id']").trigger("change");


$("#button-voucher").on("click", function () {
    $.ajax({
        url: "index.php?route=extension/total/voucher/voucher",
        type: "post",
        data:
            "voucher=" +
            encodeURIComponent(
                $("input[name='voucher']").val()
            ),
        dataType: "json",
        beforeSend: function () {
            $("#button-voucher").button("loading");
        },
        complete: function () {
            $("#button-voucher").button("reset");
        },
        success: function (json) {
            $(".alert-dismissible").remove();

            if (json["error"]) {
                $(".container").prepend(
                    '<div class="alert alert-danger alert-dismissible"><i class="fa fa-exclamation-circle"></i> ' +
                    json["error"] +
                    '<button type="button" class="close" data-dismiss="alert">&times;</button></div>'
                );

                $("html, body").animate(
                    { scrollTop: 0 },
                    "slow"
                );
            }

            if (json["redirect"]) {
                window.location = json["redirect"];
            }
        },
        error: function (
            xhr,
            ajaxOptions,
            thrownError
        ) {
            alert(
                thrownError +
                "\r\n" +
                xhr.statusText +
                "\r\n" +
                xhr.responseText
            );
        },
    });
});

// Checkout sayfasında olanlar

$('#login input').keydown(function (e) {
    if (e.keyCode == 13) {
        $('#button-login').click();
    }
});

$(document).ready(function () {
    // Sort the custom fields
    $('#custom-field-payment .custom-field[data-sort]').detach().each(function () {
        if ($(this).attr('data-sort') >= 0 && $(this).attr('data-sort') <= $('#payment-address .col-sm-6').length) {
            $('#payment-address .col-sm-6').eq($(this).attr('data-sort')).before(this);
        }

        if ($(this).attr('data-sort') > $('#payment-address .col-sm-6').length) {
            $('#payment-address .col-sm-6:last').after(this);
        }

        if ($(this).attr('data-sort') < -$('#payment-address .col-sm-6').length) {
            $('#payment-address .col-sm-6:first').before(this);
        }
    });

    $('#payment-address select[name=\'customer_group_id\']').on('change', function () {
        $.ajax({
            url: 'index.php?route=checkout/checkout/customfield&customer_group_id=' + this.value,
            dataType: 'json',
            success: function (json) {
                $('#payment-address .custom-field').hide();
                $('#payment-address .custom-field').removeClass('required');

                for (let i = 0; i < json.length; i++) {
                    let custom_field = json[i];

                    $('#payment-custom-field' + custom_field['custom_field_id']).show();

                    if (custom_field['required']) {
                        $('#payment-custom-field' + custom_field['custom_field_id']).addClass('required');
                    } else {
                        $('#payment-custom-field' + custom_field['custom_field_id']).removeClass('required');
                    }
                }


                $('#shipping-address .custom-field').hide();
                $('#shipping-address .custom-field').removeClass('required');

                for (let i = 0; i < json.length; i++) {
                    let custom_field = json[i];

                    $('#shipping-custom-field' + custom_field['custom_field_id']).show();

                    if (custom_field['required']) {
                        $('#shipping-custom-field' + custom_field['custom_field_id']).addClass('required');
                    } else {
                        $('#shipping-custom-field' + custom_field['custom_field_id']).removeClass('required');
                    }
                }

            },

        });
    });

    $('#payment-address select[name=\'customer_group_id\']').trigger('change');

    $('#payment-address button[id^=\'button-payment-custom-field\']').on('click', function () {
        var node = this;
        let timer;
        $('#form-upload').remove();

        $('body').prepend('<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>');

        $('#form-upload input[name=\'file\']').trigger('click');

        timer = setInterval(function () {
            if ($('#form-upload input[name=\'file\']').val() != '') {
                clearInterval(timer);

                $.ajax({
                    url: 'index.php?route=tool/upload',
                    type: 'post',
                    dataType: 'json',
                    data: new FormData($('#form-upload')[0]),
                    cache: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $(node).button('loading');
                    },
                    complete: function () {
                        $(node).button('reset');
                    },
                    success: function (json) {
                        $('.text-danger').remove();

                        if (json['error']) {
                            $(node).parent().find('input[name^=\'custom_field\']').after('<div class="text-danger">' + json['error'] + '</div>');
                        }

                        if (json['success']) {
                            alert(json['success']);

                            $(node).parent().find('input[name^=\'custom_field\']').attr('value', json['file']);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
                    }
                });
            }
        }, 500);
    });

    $('#payment-address select[name=\'country_id\']').on('change', function () {
        $.ajax({
            url: 'index.php?route=extension/quickcheckout/checkout/country&country_id=' + this.value,
            dataType: 'json',
            cache: false,
            beforeSend: function () {
                $('#payment-address select[name=\'country_id\']').after('<i class="fa fa-spinner fa-spin"></i>');
            },
            complete: function () {
                $('.fa-spinner').remove();
            },
            success: function (json) {
                if (json['postcode_required'] == '1') {
                    $('#payment-postcode-required').addClass('required');
                } else {
                    $('#payment-postcode-required').removeClass('required');
                }

                var html = '';

                if (json['zone'] != '') {
                    for (let i = 0; i < json['zone'].length; i++) {
                        html += '<option value="' + json['zone'][i]['zone_id'] + '"';

                        if (json['zone'][i]['zone_id'] == '') {
                            html += ' selected="selected"';
                        }

                        html += '>' + json['zone'][i]['name'] + '</option>';
                    }
                } else {
                    html += '<option value="0" selected="selected"> --- Yok --- </option>';
                }

                $('#payment-address select[name=\'zone_id\']').html(html).trigger('change');
            },

        });
    });

    $('#payment-address select[name=\'country_id\']').trigger('change');


    // Guest Shipping Form
    $('#payment-address input[name=\'shipping_address\']').on('change', function () {
        if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
            $('#shipping-address').slideUp('slow');


            reloadShippingMethod('payment');

        } else {
            $.ajax({
                url: 'index.php?route=extension/quickcheckout/guest_shipping&customer_group_id=' + $('#payment-address select[name=\'customer_group_id\']').val(),
                dataType: 'html',
                cache: false,
                beforeSend: function () {
                    // Nothing at the moment
                },
                success: function (html) {
                    $('#shipping-address .quickcheckout-content').html(html);

                    $('#shipping-address').slideDown('slow');
                },

            });
        }
    });


    $('#shipping-address').hide();


    $('#payment-address select[name=\'zone_id\']').on('change', function () {
        reloadPaymentMethod();


        if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
            reloadShippingMethod('payment');
        }

    });

    // Create account
    $('#payment-address input[name=\'create_account\']').on('change', function () {
        if ($('#payment-address input[name=\'create_account\']:checked').val()) {
            $('#create_account').slideDown('slow');
        } else {
            $('#create_account').slideUp('slow');
        }
    });


    $('#create_account').hide();
});
$('#coupon-heading').on('click', function () {
    if ($('#coupon-content').is(':visible')) {
        $('#coupon-content').slideUp('slow');
    } else {
        $('#coupon-content').slideDown('slow');
    }
});

$('#voucher-heading').on('click', function () {
    if ($('#voucher-content').is(':visible')) {
        $('#voucher-content').slideUp('slow');
    } else {
        $('#voucher-content').slideDown('slow');
    }
});

$('#reward-heading').on('click', function () {
    if ($('#reward-content').is(':visible')) {
        $('#reward-content').slideUp('slow');
    } else {
        $('#reward-content').slideDown('slow');
    }
});



$(window).load(function () {
    $.blockUI({
        message: '<h1 style="color:#ffffff;">Lütfen bekleyin...</h1>',
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            '-khtml-border-radius': '10px',
            'border-radius': '10px',
            opacity: .8,
            color: '#ffffff'
        }
    });

    setTimeout(function () {
        $.unblockUI();
    }, 2000);
});


// Save form data
$(document).on('change', '#payment-address input[type=\'text\'], #payment-address select', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/checkout/save&type=payment',
        type: 'post',
        data: $('#payment-address input[type=\'text\'], #payment-address input[type=\'password\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address input[type=\'hidden\'], #payment-address select, #payment-address textarea'),
        dataType: 'json',
        cache: false,
        success: function () {
            // No action needed
        },

    });
});

$(document).on('change', '#shipping-address input[type=\'text\'], #shipping-address select', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/checkout/save&type=shipping',
        type: 'post',
        data: $('#shipping-address input[type=\'text\'], #shipping-address input[type=\'password\'], #shipping-address input[type=\'checkbox\']:checked, #shipping-address input[type=\'radio\']:checked, #shipping-address input[type=\'hidden\'], #shipping-address select, #shipping-address textarea'),
        dataType: 'json',
        cache: false,
        success: function () {
            // No action needed
        },

    });
});



// Login Form Clicked
$(document).on('click', '#button-login', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/login/validate',
        type: 'post',
        data: $('#checkout #login :input'),
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            $('#button-login').prop('disabled', true);
            $('#button-login').button('loading');
        },
        complete: function () {
            $('#button-login').prop('disabled', false);
            $('#button-login').button('reset');
        },
        success: function (json) {
            $('.alert').remove();

            if (json['redirect']) {
                window.location = json['redirect'];
            } else if (json['error']) {
                $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                $('.alert-danger').fadeIn('slow');
            }
        },

    });
});


// Validate Register
function validateRegister() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/register/validate',
        type: 'post',
        data: $('#payment-address input[type=\'text\'], #payment-address input[type=\'password\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address input[type=\'hidden\'], #payment-address select, #payment-address textarea'),
        dataType: 'json',
        cache: false,
        success: function (json) {
            $('.alert, .text-danger').remove();

            if (json['redirect']) {
                window.location = json['redirect'];
            } else if (json['error']) {
                $('#button-payment-method').prop('disabled', false);
                $('#button-payment-method').button('reset');
                $('#terms input[type=\'checkbox\']').prop('checked', false);

                $('.fa-spinner').remove();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                if (json['error']['warning']) {
                    $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                    $('.alert-danger').fadeIn('slow');
                }


                if (json['error']['password']) {
                    $('#payment-address input[name=\'password\']').after('<div class="text-danger">' + json['error']['password'] + '</div>');
                }

                if (json['error']['confirm']) {
                    $('#payment-address input[name=\'confirm\']').after('<div class="text-danger">' + json['error']['confirm'] + '</div>');
                }


                if (json['error']['password']) {
                    $('#payment-address input[name=\'password\']').css('border', '1px solid #f00').css('background', '#F8ACAC');
                }

                if (json['error']['confirm']) {
                    $('#payment-address input[name=\'confirm\']').css('border', '1px solid #f00').css('background', '#F8ACAC');
                }

            } else {

                var shipping_address = $('#payment-address input[name=\'shipping_address\']:checked').val();

                if (shipping_address) {
                    validateShippingMethod();
                } else {
                    validateGuestShippingAddress();
                }

            }
        },

    });
}

// Validate Guest Payment Address
function validateGuestAddress() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/guest/validate',
        type: 'post',
        data: $('#payment-address input[type=\'text\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address select, #payment-address textarea'),
        dataType: 'json',
        cache: false,
        success: function (json) {
            $('.alert, .text-danger').remove();

            if (json['redirect']) {
                window.location = json['redirect'];
            } else if (json['error']) {
                $('#button-payment-method').prop('disabled', false);
                $('#button-payment-method').button('reset');
                $('#terms input[type=\'checkbox\']').prop('checked', false);

                $('.fa-spinner').remove();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                if (json['error']['warning']) {
                    $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                    $('.alert-danger').fadeIn('slow');
                }


                for (let i in json['error']) {
                    let element = $('#input-payment-' + i.replace('_', '-'));

                    if ($(element).parent().hasClass('input-group')) {
                        $(element).parent().after('<div class="text-danger">' + json['error'][i] + '</div>');
                    } else {
                        $(element).after('<div class="text-danger">' + json['error'][i] + '</div>');
                    }
                }


                for (let i in json['error']) {
                    let element = $('#input-payment-' + i.replace('_', '-'));

                    $(element).css('border', '1px solid #f00').css('background', '#F8ACAC');
                }

            } else {
                var create_account = $('#payment-address input[name=\'create_account\']:checked').val();


                var shipping_address = $('#payment-address input[name=\'shipping_address\']:checked').val();

                if (create_account) {
                    validateRegister();
                } else {
                    if (shipping_address) {
                        validateShippingMethod();
                    } else {
                        validateGuestShippingAddress();
                    }
                }
            }
        },

    });
}

// Validate Guest Shipping Address
function validateGuestShippingAddress() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/guest_shipping/validate',
        type: 'post',
        data: $('#shipping-address input[type=\'text\'], #shipping-address input[type=\'checkbox\']:checked, #shipping-address input[type=\'radio\']:checked, #shipping-address select, #shipping-address textarea'),
        dataType: 'json',
        cache: false,
        success: function (json) {
            $('.alert, .text-danger').remove();

            if (json['redirect']) {
                window.location = json['redirect'];
            } else if (json['error']) {
                $('#button-payment-method').prop('disabled', false);
                $('#button-payment-method').button('reset');
                $('#terms input[type=\'checkbox\']').prop('checked', false);

                $('.fa-spinner').remove();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                if (json['error']['warning']) {
                    $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                    $('.alert-danger').fadeIn('slow');
                }


                for (let i in json['error']) {
                    let element = $('#input-shipping-' + i.replace('_', '-'));

                    if ($(element).parent().hasClass('input-group')) {
                        $(element).parent().after('<div class="text-danger">' + json['error'][i] + '</div>');
                    } else {
                        $(element).after('<div class="text-danger">' + json['error'][i] + '</div>');
                    }
                }


                for (let i in json['error']) {
                    let element = $('#input-shipping-' + i.replace('_', '-'));

                    $(element).css('border', '1px solid #f00').css('background', '#F8ACAC');
                }

            } else {
                validateShippingMethod();
            }
        },

    });
}

// Confirm Payment
$(document).on('click', '#button-payment-method', function () {
    $('#button-payment-method').prop('disabled', true);
    $('#button-payment-method').button('loading');

    $('#button-payment-method').after('<i class="fa fa-spinner fa-spin"></i>');

    validateGuestAddress();
});
// Close if logged php

// Payment Method
function reloadPaymentMethod() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/payment_method',
        type: 'post',
        data: $('#payment-address input[type=\'text\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address input[type=\'hidden\'], #payment-address select, #payment-address textarea, #payment-method input[type=\'text\'], #payment-method input[type=\'checkbox\']:checked, #payment-method input[type=\'radio\']:checked, #payment-method input[type=\'hidden\'], #payment-method select, #payment-method textarea'),
        dataType: 'html',
        cache: false,
        beforeSend: function () {
            moduleLoad($('#payment-method'), true);
        },
        success: function (html) {
            moduleLoaded($('#payment-method'), true);

            $('#payment-method .quickcheckout-content').html(html);


            $.unblockUI();

        },

    });
}

function reloadPaymentMethodById(address_id) {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/payment_method&address_id=' + address_id,
        type: 'post',
        data: $('#payment-method input[type=\'checkbox\']:checked, #payment-method input[type=\'radio\']:checked, #payment-method input[type=\'hidden\'], #payment-method select, #payment-method textarea'),
        dataType: 'html',
        cache: false,
        beforeSend: function () {
            moduleLoad($('#payment-method'), true);
        },
        success: function (html) {
            moduleLoaded($('#payment-method'), true);

            $('#payment-method .quickcheckout-content').html(html);


            $.unblockUI();

        },

    });
}

// Validate Payment Method
function validatePaymentMethod() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/payment_method/validate',
        type: 'post',
        data: $('#payment-method select, #payment-method input[type=\'radio\']:checked, #payment-method input[type=\'checkbox\']:checked, #payment-method textarea'),
        dataType: 'json',
        cache: false,
        success: function (json) {
            $('.alert, .text-danger').remove();

            if (json['redirect']) {
                window.location = json['redirect'];
            } else if (json['error']) {
                $('#button-payment-method').prop('disabled', false);
                $('#button-payment-method').button('reset');
                $('#terms input[type=\'checkbox\']').prop('checked', false);

                $('.fa-spinner').remove();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                if (json['error']['warning']) {
                    $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                    $('.alert-danger').fadeIn('slow');
                }
            } else {
                validateTerms();
            }
        },

    });
}

// Shipping Method

function reloadShippingMethod(type) {
    var post_data;
    if (type == 'payment') {
         post_data = $('#payment-address input[type=\'text\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address input[type=\'hidden\'], #payment-address select, #payment-address textarea, #shipping-method input[type=\'text\'], #shipping-method input[type=\'checkbox\']:checked, #shipping-method input[type=\'radio\']:checked, #shipping-method input[type=\'hidden\'], #shipping-method select, #shipping-method textarea');
    } else {
         post_data = $('#shipping-address input[type=\'text\'], #shipping-address input[type=\'checkbox\']:checked, #shipping-address input[type=\'radio\']:checked, #shipping-address input[type=\'hidden\'], #shipping-address select, #shipping-address textarea, #shipping-method input[type=\'text\'], #shipping-method input[type=\'checkbox\']:checked, #shipping-method input[type=\'radio\']:checked, #shipping-method input[type=\'hidden\'], #shipping-method select, #shipping-method textarea');
    }

    $.ajax({
        url: 'index.php?route=extension/quickcheckout/shipping_method',
        type: 'post',
        data: post_data,
        dataType: 'html',
        cache: false,
        beforeSend: function () {
            moduleLoad($('#shipping-method'), true);
        },
        success: function (html) {
            moduleLoaded($('#shipping-method'), true);

            $('#shipping-method .quickcheckout-content').html(html);
        },

    });
}

function reloadShippingMethodById(address_id) {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/shipping_method&address_id=' + address_id,
        type: 'post',
        data: $('#shipping-method input[type=\'text\'], #shipping-method input[type=\'checkbox\']:checked, #shipping-method input[type=\'radio\']:checked, #shipping-method input[type=\'hidden\'], #shipping-method select, #shipping-method textarea'),
        dataType: 'html',
        cache: false,
        beforeSend: function () {
            moduleLoad($('#shipping-method'), true);
        },
        success: function (html) {
            moduleLoaded($('#shipping-method'), true);

            $('#shipping-method .quickcheckout-content').html(html);
        },

    });
}

// Validate Shipping Method
function validateShippingMethod() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/shipping_method/validate',
        type: 'post',
        data: $('#shipping-method select, #shipping-method input[type=\'radio\']:checked, #shipping-method textarea, #shipping-method input[type=\'text\']'),
        dataType: 'json',
        cache: false,
        success: function (json) {
            $('.alert, .text-danger').remove();

            if (json['redirect']) {
                window.location = json['redirect'];
            } else if (json['error']) {
                $('#button-payment-method').prop('disabled', false);
                $('#button-payment-method').button('reset');
                $('#terms input[type=\'checkbox\']').prop('checked', false);

                $('.fa-spinner').remove();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                if (json['error']['warning']) {
                    $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                    $('.alert-danger').fadeIn('slow');
                }
            } else {
                validatePaymentMethod();
            }
        },

    });
}


// Validate confirm button
function validateTerms() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/terms/validate',
        type: 'post',
        data: $('#terms input[type=\'checkbox\']:checked'),
        dataType: 'json',
        cache: false,
        success: function (json) {
            if (json['redirect']) {
                window.location = json['redirect'];
            }

            if (json['error']) {
                $('#button-payment-method').prop('disabled', false);
                $('#button-payment-method').button('reset');
                $('#terms input[type=\'checkbox\']').prop('checked', false);

                $('.fa-spinner').remove();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

                if (json['error']['warning']) {
                    $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                    $('.alert-danger').fadeIn('slow');
                }
            } else {
                loadConfirm();
            }
        },

    });
}

// Load confirm
function loadConfirm() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/confirm',
        dataType: 'html',
        cache: false,
        beforeSend: function () {

            $('html, body').animate({ scrollTop: 0 }, 'slow');


            $('#quickcheckoutconfirm').html('<div class="text-center"><i class="fa fa-spinner fa-spin fa-5x"></i></div>');



            $.blockUI({
                message: '<h1 style="color:#ffffff;">Lütfen bekleyin...</h1>',
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    '-khtml-border-radius': '10px',
                    'border-radius': '10px',
                    opacity: .8,
                    color: '#ffffff'
                }
            });

        },
        success: function (html) {


            $.unblockUI();


            $('#quickcheckoutconfirm').hide().html(html);



            $('#quickcheckoutconfirm').show();

        },

    });
}

// Load cart

function loadCart() {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/cart',
        dataType: 'html',
        cache: false,
        beforeSend: function () {
            $('.tooltip').remove();

            moduleLoad($('#cart1'), true);
        },
        success: function (html) {
            moduleLoaded($('#cart1'), true);

            $('#cart1 .quickcheckout-content').html(html);
        },

    });
}





// Validate Coupon
$(document).on('click', '#button-coupon', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/voucher/validateCoupon',
        type: 'post',
        data: $('#coupon-content :input'),
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            $('#button-coupon').prop('disabled', true);
            $('#button-coupon').after('<i class="fa fa-spinner fa-spin"></i>');
        },
        complete: function () {
            $('#button-coupon').prop('disabled', false);
            $('#coupon-content .fa-spinner').remove();
        },
        success: function (json) {
            $('.alert').remove();

            $('html, body').animate({ scrollTop: 0 }, 'slow');

            if (json['success']) {
                $('#success-messages').prepend('<div class="alert alert-success" style="display:none;"><i class="fa fa-check-circle"></i> ' + json['success'] + '</div>');

                $('.alert-success').fadeIn('slow');
            } else if (json['error']) {
                $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                $('.alert-danger').fadeIn('slow');
            }


            if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
                reloadPaymentMethod();


                reloadShippingMethod('payment');

            } else {
                reloadPaymentMethod();


                reloadShippingMethod('shipping');

            }



        },

    });
});

$(document).on('click', '#button-voucher', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/voucher/validateVoucher',
        type: 'post',
        data: $('#voucher-content :input'),
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            $('#button-voucher').prop('disabled', true);
            $('#button-voucher').after('<i class="fa fa-spinner fa-spin"></i>');
        },
        complete: function () {
            $('#button-voucher').prop('disabled', false);
            $('#voucher-content .fa-spinner').remove();
        },
        success: function (json) {
            $('.alert').remove();

            $('html, body').animate({ scrollTop: 0 }, 'slow');

            if (json['success']) {
                $('#success-messages').prepend('<div class="alert alert-success" style="display:none;"><i class="fa fa-check-circle"></i> ' + json['success'] + '</div>');

                $('.alert-success').fadeIn('slow');
            } else if (json['error']) {
                $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                $('.alert-danger').fadeIn('slow');
            }


            if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
                reloadPaymentMethod();


                reloadShippingMethod('payment');

            } else {
                reloadPaymentMethod();


                reloadShippingMethod('shipping');

            }


        },

    });
});

$(document).on('click', '#button-reward', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/voucher/validateReward',
        type: 'post',
        data: $('#reward-content :input'),
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            $('#button-reward').prop('disabled', true);
            $('#button-reward').after('<i class="fa fa-spinner fa-spin"></i>');
        },
        complete: function () {
            $('#button-reward').prop('disabled', false);
            $('#reward-content .fa-spinner').remove();
        },
        success: function (json) {
            $('.alert').remove();

            $('html, body').animate({ scrollTop: 0 }, 'slow');

            if (json['success']) {
                $('#success-messages').prepend('<div class="alert alert-success" style="display:none;"><i class="fa fa-check-circle"></i> ' + json['success'] + '</div>');

                $('.alert-success').fadeIn('slow');
            } else if (json['error']) {
                $('#warning-messages').prepend('<div class="alert alert-danger" style="display: none;"><i class="fa fa-exclamation-circle"></i> ' + json['error']['warning'] + '</div>');

                $('.alert-danger').fadeIn('slow');
            }


            if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
                reloadPaymentMethod();


                reloadShippingMethod('payment');

            } else {
                reloadPaymentMethod();


                reloadShippingMethod('shipping');

            }


        },

    });
});


$(document).on('focusout', 'input[name=\'postcode\']', function () {

    if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
        reloadShippingMethod('payment');
    } else {
        reloadShippingMethod('shipping');
    }

});


$(document).on('keydown', 'input', function () {
    $(this).css('background', '').css('border', '');

    $(this).siblings('.text-danger').remove();
});
$(document).on('change', 'select', function () {
    $(this).css('background', '').css('border', '');

    $(this).siblings('.text-danger').remove();
});



$(document).on('click', '.button-update', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/cart/update',
        type: 'post',
        data: $('#cart1 :input'),
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            $('#cart1 .button-update').prop('disabled', true);
        },
        success: function (json) {
            if (json['redirect']) {
                window.location = json['redirect'];
            } else {

                if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
                    reloadPaymentMethod();


                    reloadShippingMethod('payment');

                } else {
                    reloadPaymentMethod();


                    reloadShippingMethod('shipping');

                }



            }
        },

    });
});

$(document).on('click', '.button-remove', function () {
    var remove_id = $(this).attr('data-remove');

    $.ajax({
        url: 'index.php?route=extension/quickcheckout/cart/update&remove=' + remove_id,
        type: 'get',
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            $('#cart1 .button-remove').prop('disabled', true);
        },
        success: function (json) {
            if (json['redirect']) {
                window.location = json['redirect'];
            } else {

                if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
                    reloadPaymentMethod();


                    reloadShippingMethod('payment');

                } else {
                    reloadPaymentMethod();


                    reloadShippingMethod('shipping');

                }


            }
        },

    });

    return false;
});

$('.date').datetimepicker({
    format: 'YYYY-MM-DD'
});

$('.time').datetimepicker({
    format: 'HH:mm'
});

$('.datetime').datetimepicker();

$('#payment-method input[name=\'payment_method\'], #payment-method select[name=\'payment_method\']').on('change', function () {
    $.ajax({
        url: 'index.php?route=extension/quickcheckout/payment_method/set',
        type: 'post',
        data: $('#payment-address input[type=\'text\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address input[type=\'hidden\'], #payment-address select, #payment-method input[type=\'text\'], #payment-method input[type=\'checkbox\']:checked, #payment-method input[type=\'radio\']:checked, #payment-method input[type=\'hidden\'], #payment-method select, #payment-method textarea'),
        dataType: 'html',
        cache: false,
        success: function () {

        },

    });
});


$('#shipping-method input[name=\'shipping_method\'], #shipping-method select[name=\'shipping_method\']').on('change', function () {
    var post_data;
    if ($('#payment-address input[name=\'shipping_address\']:checked').val()) {
         post_data = $('#payment-address input[type=\'text\'], #payment-address input[type=\'checkbox\']:checked, #payment-address input[type=\'radio\']:checked, #payment-address input[type=\'hidden\'], #payment-address select, #shipping-method input[type=\'text\'], #shipping-method input[type=\'checkbox\']:checked, #shipping-method input[type=\'radio\']:checked, #shipping-method input[type=\'hidden\'], #shipping-method select, #shipping-method textarea');
    } else {
         post_data = $('#shipping-address input[type=\'text\'], #shipping-address input[type=\'checkbox\']:checked, #shipping-address input[type=\'radio\']:checked, #shipping-address input[type=\'hidden\'], #shipping-address select, #shipping-method input[type=\'text\'], #shipping-method input[type=\'checkbox\']:checked, #shipping-method input[type=\'radio\']:checked, #shipping-method input[type=\'hidden\'], #shipping-method select, #shipping-method textarea');
    }

    $.ajax({
        url: 'index.php?route=extension/quickcheckout/shipping_method/set',
        type: 'post',
        data: post_data,
        dataType: 'html',
        cache: false,
        success: function () {

            loadCart();
        },

    });
});

$(document).ready(function () {
    $('#shipping-method input[name=\'shipping_method\']:checked, #shipping-method select[name=\'shipping_method\']').trigger('change');
});


// Shipping address form functions
$(document).ready(function () {
    $('#shipping-address input[name=\'shipping_address\']').on('change', function () {
        if (this.value == 'new') {
            $('#shipping-existing').slideUp();
            $('#shipping-new').slideDown();

            setTimeout(function () {
                $('#shipping-address select[name=\'country_id\']').trigger('change');
            }, 200);
        } else {
            $('#shipping-existing').slideDown();
            $('#shipping-new').slideUp();

            reloadShippingMethodById($('#shipping-address select[name=\'address_id\']').val());
        }
    });

    $('#shipping-address input[name=\'shipping_address\']:checked').trigger('change');

    // Sort the custom fields
    $('#custom-field-shipping .custom-field[data-sort]').detach().each(function () {
        if ($(this).attr('data-sort') >= 0 && $(this).attr('data-sort') <= $('#shipping-new .col-sm-6').length) {
            $('#shipping-new .col-sm-6').eq($(this).attr('data-sort')).before(this);
        }

        if ($(this).attr('data-sort') > $('#shipping-new .col-sm-6').length) {
            $('#shipping-new .col-sm-6:last').after(this);
        }

        if ($(this).attr('data-sort') < -$('#shipping-new .col-sm-6').length) {
            $('#shipping-new .col-sm-6:first').before(this);
        }
    });

    $('#shipping-address button[id^=\'button-shipping-custom-field\']').on('click', function () {
        var node = this;
        let timer;
        $('#form-upload').remove();

        $('body').prepend('<form enctype="multipart/form-data" id="form-upload" style="display: none;"><input type="file" name="file" /></form>');

        $('#form-upload input[name=\'file\']').trigger('click');

        timer = setInterval(function () {
            if ($('#form-upload input[name=\'file\']').val() != '') {
                clearInterval(timer);

                $.ajax({
                    url: 'index.php?route=tool/upload',
                    type: 'post',
                    dataType: 'json',
                    data: new FormData($('#form-upload')[0]),
                    cache: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $(node).button('loading');
                    },
                    complete: function () {
                        $(node).button('reset');
                    },
                    success: function (json) {
                        $('.text-danger').remove();

                        if (json['error']) {
                            $(node).parent().find('input[name^=\'custom_field\']').after('<div class="text-danger">' + json['error'] + '</div>');
                        }

                        if (json['success']) {
                            alert(json['success']);

                            $(node).parent().find('input[name^=\'custom_field\']').attr('value', json['file']);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
                    }
                });
            }
        }, 500);
    });

    $('#shipping-address select[name=\'zone_id\']').on('change', function () {
        reloadShippingMethod('shipping');
    });

    $('#shipping-address select[name=\'country_id\']').on('change', function () {
        $.ajax({
            url: 'index.php?route=extension/quickcheckout/checkout/country&country_id=' + this.value,
            dataType: 'json',
            cache: false,
            beforeSend: function () {
                $('#shipping-address select[name=\'country_id\']').after('<i class="fa fa-spinner fa-spin"></i>');
            },
            complete: function () {
                $('.fa-spinner').remove();
            },
            success: function (json) {
                if (json['postcode_required'] == '1') {
                    $('#shipping-postcode-required').addClass('required');
                } else {
                    $('#shipping-postcode-required').removeClass('required');
                }

                let html = '';

                if (json['zone'] != '') {
                    for (let i = 0; i < json['zone'].length; i++) {
                        html += '<option value="' + json['zone'][i]['zone_id'] + '"';

                        if (json['zone'][i]['zone_id'] == '3372 ') {
                            html += ' selected="selected"';
                        }

                        html += '>' + json['zone'][i]['name'] + '</option>';
                    }
                } else {
                    html += '<option value="0" selected="selected"> --- Yok --- </option>';
                }

                $('#shipping-address select[name=\'zone_id\']').html(html).trigger('change');
            },

        });
    });

    $('#shipping-address select[name=\'address_id\']').on('change', function () {
        if ($('#shipping-address input[name=\'shipping_address\']:checked').val() == 'new') {
            reloadShippingMethod('shipping');
        } else {
            reloadShippingMethodById($('#shipping-address select[name=\'address_id\']').val());
        }
    });
});
