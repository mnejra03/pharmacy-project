namespace NewPharmacy.Endpoints.DataSeedEndpoints;

using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Data.Models;
using NewPharmacy.Data;
using NewPharmacy.Helper.Api;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;

[Route("data-seed")]
public class DataSeedGenerateEndpoint(ApplicationDbContext db)
    : MyEndpointBaseAsync
    .WithoutRequest
    .WithResult<string>
{
    [HttpPost]
    [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
    public override async Task<string> HandleAsync(CancellationToken cancellationToken = default)
    {

        var existingCategories = db.Categories.ToList();
        
        var requiredCategoryNames = new List<string>
        {
            "Your health",
            "Beauty and care",
            "Childcare",
            "Skin protection",
            "Devices"
        };
        foreach (var catName in requiredCategoryNames)
        {
            if (!existingCategories.Any(c => c.Name == catName))
            {
                var newCategory = new Category { Name = catName };
                db.Categories.Add(newCategory);
                existingCategories.Add(newCategory);
            }
        }


        await db.SaveChangesAsync(cancellationToken); 

        var categories = requiredCategoryNames
            .Select(name => existingCategories.FirstOrDefault(c => c.Name == name))
            .ToList();

        var existingBrands = db.Brands.ToList();

        var requiredBrands = new Dictionary<string, (string LogoUrl, string Description)>
        {
            { "Flobian",
            ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (2).png",
            "Flobian is a well-known brand in the pharmaceutical and healthcare industry, specializing in the production and distribution of products that support health, particularly in the area of cardiovascular issues.")},
            { "Yasenka", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/yasenka-logo.jpg",
            "Yasenka is a reputable brand in the pharmaceutical industry, known for its production of high-quality medicines and health products. The brand specializes in offering a wide range of products aimed at improving general health and well-being, including over-the-counter medications, dietary supplements, and other health-related products. Yasenka is recognized for its commitment to providing effective solutions to a variety of health concerns, with a focus on quality and safety. If you'd like more detailed information about specific products they offer, feel free to ask!\r\n")},
            { "Cydonia", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/images.png",
            "Cydonia is a Bosnian brand that specializes in the production of herbal medicines, dietary supplements, and cosmetics. Founded in 1996 and based in Gračanica, Cydonia is known for using locally sourced plant materials from the untouched regions of Bosnia and Herzegovina. The brand offers products for immunity, bone health, urinary tract health, calming effects, as well as skincare and child care. Cydonia also exports its products to countries such as Ireland, the Netherlands, Ukraine, Azerbaijan, and Kuwait.")},
            { "Medisana", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download.png",
            "Medisana is a global brand recognized for providing health and wellness products, especially in the field of personal healthcare. Their product range includes devices for monitoring vital signs, blood pressure, body temperature, and weight, as well as wellness products like massagers and air purifiers. Medisana focuses on enhancing the well-being and quality of life of its customers through advanced, reliable, and user-friendly health devices.")},
            { "Humana", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/348s.png",
            "Humana is a well-known brand specializing in baby nutrition. They offer a range of products such as infant formulas, cereals, and snacks, designed to support healthy growth. With over 65 years of experience, Humana focuses on high-quality, natural ingredients and scientific research to meet the nutritional needs of babies and toddlers. All products are manufactured in Germany, ensuring safety and quality.\r\n")},
            { "Beurer", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/images (1).png",
            "Beurer is a German brand specializing in health and wellness products, founded in 1919. They offer a wide range of items, including blood pressure monitors, heating pads, massage devices, fitness trackers, and baby care products. Beurer is known for high-quality, innovative products and has received multiple awards for brand excellence.")},
            { "CeraVe", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (1).png",
            "CeraVe is a skincare brand developed by dermatologists, known for affordable and effective products. Launched in 2005, it focuses on restoring the skin's natural barrier with ceramide-rich formulas and 24-hour hydration using MVE technology. Recommended by dermatologists, CeraVe offers solutions for acne, eczema, and dry skin and is available in over 35 countries.")},
            { "Avene", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (3).png",
            "Avène is a French skincare brand focused on sensitive skin, using Avène Thermal Spring Water known for its soothing properties. Founded in 1990, it's dermatologist-recommended for conditions like eczema, acne, and rosacea. Avène offers a range of products, including cleansers, moisturizers, and sunscreens, and is available in over 100 countries.")},
            { "Clinique", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (4).png",
            "Clinique is a dermatologist-developed skincare and cosmetics brand launched in 1968 by Carol Phillips and Dr. Norman Orentreich. Known for its allergy-tested and fragrance-free products, Clinique offers a range of skincare and makeup solutions tailored to various skin types and concerns. The brand's signature 3-Step Skincare System—cleanse, exfoliate, and moisturize—remains a cornerstone of its product lineup. Clinique is committed to sustainability, aiming for 75–100% of its packaging to be recyclable, refillable, reusable, recycled, or recoverable by 2025. ")},
            { "A-derma", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (5).png",
            "A-Derma is a French dermo-cosmetic brand founded in 1982, known for using Rhealba® oats, which have soothing properties. Their products are plant-based, free of parabens, alcohol, and fragrances. A-Derma offers solutions for sensitive skin, including care for dryness, atopic dermatitis, irritations, scars, and sun protection. It’s dermatologically tested and suitable for the whole family.") },
            { "Biomd", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (6).png",
            "BioMD is a German pharmaceutical company specializing in dermocosmetics. Founded in 2010, BioMD combines natural ingredients with clinical efficacy to offer high-quality skincare solutions.") },
            { "Herbiko", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (7).png",
            "Herbiko is a Bosnian brand offering herbal-based products aimed at supporting respiratory and immune health. Their product range includes syrups for adults and children, nasal sprays, and dietary supplements. Herbiko products are available in pharmacies across Bosnia and Herzegovina and neighboring countries.")},
            { "Natures finest", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (8).png",
            "Nature’s Finest is a Slovenian brand specializing in natural dietary supplements and functional foods. Founded in 2010 by Bojan Kržič and renowned nutrition expert Michel Montignac, the company emphasizes transparency, sustainability, and quality.")},
            { "Advancis", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (9).png",
            "Advancis is a Portuguese pharmaceutical brand specializing in nutraceuticals and dietary supplements. Established by Farmodiética in 1985, Advancis offers a comprehensive range of products aimed at enhancing family well-being, including solutions for immune support, energy, vitality, hair care, skin health, and weight management. Their offerings encompass ready-to-market products, regulatory support, and tailored marketing materials. Advancis ensures high-quality standards through certifications such as GMP, ISO, and FDA compliance.") },
            { "Melem", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (10).png",
            "Melem is a traditional herbal ointment from the Balkans, notably produced in Bosnia and Herzegovina. Its formulation includes lanolin, beeswax, castor oil, and petroleum jelly, making it effective for treating dry, cracked, or irritated skin. Melem is widely used for conditions such as eczema, minor burns, insect bites, and sunburns. It is dermatologically tested, free from preservatives, silicones, and corticosteroids, and is not tested on animals .")},
            { "Gloria", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/images.jpeg",
            "Gloria is a Bosnian natural cosmetics brand founded in 2014 by Gloria Galić-Volarić. Inspired by her background in phytotherapy and a desire to create high-quality, cruelty-free skincare, Gloria developed a line of products free from synthetic preservatives, parabens, SLS, formaldehyde, fragrances, dyes, and silicones .")},
            { "Vichy", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (11).png",
            "Vichy is a renowned French skincare brand that combines dermatological expertise with the power of volcanic water, rich in minerals. Their products focus on sensitive skin, addressing concerns such as aging, hydration, and skin protection. Vichy’s offerings include facial care, anti-aging treatments, and products that enhance skin health. The brand is well known for its clinical approach to skincare, offering scientifically-backed solutions for everyday skin care needs.")},
            { "Ecodenta", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (12).png",
            "Ecodenta is a Lithuanian brand that specializes in natural oral care products. Their range includes toothpastes, mouthwashes, and other dental accessories, all made from eco-friendly and sustainable ingredients. Ecodenta products are designed to promote healthy teeth and gums without the use of harsh chemicals. The brand emphasizes environmental responsibility by using recyclable packaging and offering products that are gentle yet effective for everyday oral hygiene.") },
            { "MAM", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/images (2).png",
            "MAM is a German brand offering innovative baby products, including bottles, pacifiers, and breast-feeding accessories. Their products are designed with both functionality and baby comfort in mind, often featuring ergonomic designs and easy-to-use features. MAM is known for creating high-quality, safe, and BPA-free products for babies, focusing on supporting healthy development. The brand also integrates pediatric research into the development of its products to ensure they meet the needs of growing children.")},
            { "Trudy", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (13).png",
            "Trudy is a Croatian brand that offers a variety of dermatological products, including body lotions, creams, and sunscreens. Their products are formulated to hydrate, nourish, and protect the skin, with an emphasis on moisturizing dry and sensitive skin. Trudy’s range includes solutions for skin irritation, sun protection, and general skincare. The brand is known for using high-quality ingredients to create products that are both effective and gentle.")},
            { "Mapez", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (14).png",
            " Mapez is a Croatian brand offering a range of health products, including medical devices and natural supplements. Their products are designed to promote overall well-being, focusing on natural ingredients and alternative therapies. Mapez combines scientific research with nature’s healing properties to offer solutions for various health concerns. The brand is recognized for providing high-quality products that support a healthy lifestyle and help maintain balance.") },
            { "Hansaplast", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (15).png",
            "Hansaplast, a German brand, is known for its comprehensive range of first aid and wound care products. From bandages and plasters to advanced wound dressings, Hansaplast focuses on fast, effective healing. Their products are designed to protect wounds from infection, promote faster recovery, and minimize scarring. The brand also offers specialized products for diabetic skin care and other medical needs, backed by decades of healthcare expertise.")},
            { "BABE", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/images (3).png",
            " BABE is a Spanish skincare brand that focuses on sensitive skin, offering dermatologically tested solutions for everyday care. Their product line includes cleansers, moisturizers, sunscreens, and treatments for various skin concerns. BABE’s products are designed for people with delicate or allergic skin, using gentle, hypoallergenic ingredients. The brand is committed to providing effective skincare that’s safe for all skin types, including babies and those with sensitive conditions.\r\n\r\n")},
            { "Uriage", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (16).png",
            "Uriage is a French brand that uses thermal water from the French Alps in its skincare products to hydrate, soothe, and protect the skin. Their product range includes facial care, body lotions, and sun protection, all designed to combat environmental stressors. Uriage’s unique selling point is its use of mineral-rich thermal water that helps strengthen the skin’s natural barriers. Their formulations are dermatologist-tested, making them suitable for all skin types, including the most sensitive.")},
            { "Eliksir", ("https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/download (17).png",
            "Eliksir is a Bosnian brand specializing in natural, herbal-based skincare products. They offer a range of creams, serums, and oils that promote skin rejuvenation, protection, and health. Eliksir combines traditional herbal knowledge with modern skincare science to create products that are effective and gentle. The brand is known for its use of locally sourced, natural ingredients to provide solutions for various skin concerns.")},

        };


        foreach (var kvp in requiredBrands)
        {
            var brandName = kvp.Key;
            var logoUrl = kvp.Value.LogoUrl;
            var description = kvp.Value.Description;

            if (!existingBrands.Any(b => b.Name == brandName))
            {
                var newBrand = new Brand
                {
                    Name = brandName,
                    LogoUrl = logoUrl,
                    Description = description,
                };
                db.Brands.Add(newBrand);
                existingBrands.Add(newBrand);
            }
        }



        await db.SaveChangesAsync(cancellationToken);
        
        existingBrands = await db.Brands.ToListAsync(cancellationToken);
        existingCategories = await db.Categories.ToListAsync(cancellationToken);

       
        var brands = requiredBrands.Keys
            .Select(name => existingBrands.FirstOrDefault(b => b.Name == name))
            .ToList();

        
        var products = new List<Product>
        {
            new Product {
                Name = "Flobian capsules A10",
                Description = "Flobian® is a dietary supplement that helps with irritable bowel syndrome (stomach gas, flatulence, stomach pain, irregular bowel movements), and it is extremely successfully used throughout Europe and America as a natural, completely safe and thoroughly tested product.",
                Price = 18.30m,
                QuantityInStock = 27,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/flobian_kapsule_a10-400x400.jpg",
                Category = categories[0],
                IsDiscounted = true,
                DiscountPercentage = 10,
                DatumDodavanja = DateTime.Now,
                Brand = brands[0],
                ExpiryDate = new DateTime(2025, 12, 31)},
            new Product {
                Name = "Clinique PopTM Lip + Cheek Oil Black Honey 7ml",
                Description = "Clinique Pop Lip + Cheek Oil in Black Honey – a multi-purpose tinted oil that delivers the fresh, dewy glow of our iconic Black Honey shade to lips and cheeks.",
                Price = 59.60m,
                QuantityInStock = 10,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/CL BLACK HONEY LIP OIL-400x400.jpg",
                Category = categories[0],
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[8],
                ExpiryDate = new DateTime(2025, 11, 30)},
            new Product {
                Name = "Herbiko Propomucil syrup for children 120ml",
                Description = "Propomucil syrup for children is the only one that contains natural propolis purified by innovative technology, into which natural N-acetylcysteine (NAC) is incorporated, which breaks down the secretion and throws it out.",
                Category = categories[0],
                Price = 10.70m,
                QuantityInStock = 30,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/2020095-228x228.jpg",
                IsDiscounted = true,
                DiscountPercentage = 12,
                DatumDodavanja = DateTime.Now,
                Brand = brands[11],
                ExpiryDate = new DateTime(2025, 10, 31)},
            new Product {
                Name = "Vitamin C 1000g Powder 150g",
                Description = "Vitamin C is a powerful antioxidant, supporting the immune and nervous systems. It is recommended for athletes and recreationists, for everyone who wants to strengthen the immune system and for everyone who wants to raise their energy level.",
                Category = categories[0],
                Price = 12.90m,
                QuantityInStock = 22,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Vitamin-C-1000mg-u-prahu-150g-500x500.jpg",
                IsDiscounted = true,
                DiscountPercentage = 14,
                DatumDodavanja = DateTime.Now,
                Brand = brands[12],
                ExpiryDate = new DateTime(2025, 09, 30)},
            new Product {
                Name = "Arnika gel 250ml Cydonia",
                Description = "The ingredients act as analgesics (reduce the intensity of pain), anti-inflammatory, immunostimulating and as astringents, reducing swelling and pain in case of injuries and consequences of accidents such as, for example. hematomas, dislocations, contusions, edema due to fractures, rheumatic problems in muscles and joints.",
                Category = categories[0],
                Price = 6.10m,
                QuantityInStock = 14,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/arnika-gel-tuba-100ml-Cydonia-228x228.jpg",
                IsDiscounted = true,
                DiscountPercentage = 40,
                DatumDodavanja = DateTime.Now,
                Brand = brands[2],
                ExpiryDate = new DateTime(2025, 12, 20)},
            new Product {
                Name = "Advancis Throat spray 20ml",
                Description = "To relieve the symptoms of a sore and inflamed throat.",
                Category = categories[0],
                Price = 18.80m,
                QuantityInStock = 33,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Advancis-Throat-Sprej-20-ml-apoteka-monis.png-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[13],
                ExpiryDate = new DateTime(2025, 11, 20)},
            new Product {
                Name = "Melem original 10ml",
                Description = "Melem is an original cream that loves healthy skin and, with daily use, allows your skin to be hydrated, nourished and resistant to external influences.",
                Category = categories[0],
                Price = 6.20m,
                QuantityInStock = 20,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/2040096-400x400.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[14],
                ExpiryDate = new DateTime(2025, 11, 20)},
            new Product {
                Name = "A-DERMA Shower gel 500ml",
                Description = "It cleans, hydrates and protects the fragile skin of children (older than 2 years) and adults. It contains a gentle cleansing base and balancing ingredients.",
                Category = categories[1],
                Price = 34.80m,
                QuantityInStock = 20,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/A-Derma-gel-za-tusiranje-500-ml-Super-Apoteka-228x228.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[9],
                ExpiryDate = new DateTime(2025, 12, 20)
            },
            new Product {
                Name = "Gloria scalp peeling 100ml ",
                Description = "Scalp peeling is specially designed for oily scalp and dandruff-prone hair.",
                Category = categories[1],
                Price = 17.60m,
                QuantityInStock = 22,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Gloria-Piling-za-vlasiste-Super-Apoteka-500x500.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[15],
                ExpiryDate = new DateTime(2025, 12, 31)
                },
            new Product {
                Name = "CeraVe oil control-cream 52ml + floaming gel 236ml ",
                Description = "For combination to oily skin• Helps balance oily skin",
                Category = categories[1],
                Price = 43.00m,
                QuantityInStock = 14,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/CERAVE SET-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[6],
                ExpiryDate = new DateTime(2025, 10, 20)
                },
            new Product {
                Name = "BIOMD First Aid Face Cream 40ml",
                Description = "First Aid Face Cream is an organic, natural and hypoallergenic face cream that is great for sensitive skin with a burning and hot sensation.",
                Category = categories[1],
                Price = 34.90m,
                QuantityInStock = 30,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/BIOMD-First-aid-krema-za-lice-40ml-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[10],
                ExpiryDate = new DateTime(2025, 09, 20)
                },
            new Product {
                Name = "Neven Gel 100ml Cydonia",
                Description = "Marigold flower tincture (Calendula officinalis), as well as rose geranium oil (Pelargonium roseum), have an analgesic effect (reduce the intensity of pain)",
                Category = categories[1],
                Price = 3.00m,
                QuantityInStock = 19,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/neven-gel-tuba-100ml-Cydonia-228x228.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[2],
                ExpiryDate = new DateTime(2025, 12, 15)
                },
            new Product {
                Name = "VICHY Capital Soleil Baby Milk SPF50+ 300ml",
                Description = "High protection for children in a large package\r\nFor fair-skinned children.To combat the harmful effects of UV rays",
                Category = categories[1],
                Price = 47.10m,
                QuantityInStock = 20,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/3337871323639_1-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[16],
                ExpiryDate = new DateTime(2025, 12, 15)

                },
            new Product {
                Name = "ECODENTA Children's toothbrush Soft A1 ",
                Description = "Toothbrush designed for daily care of milk and permanent teeth of children from 1 to 7 years of age.",
                Category = categories[2],
                Price = 09.10m,
                QuantityInStock = 19, Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Ecodenta-Djecija-cetkica-soft-A1-228x228.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[17],
                ExpiryDate = new DateTime(2025, 11, 15)
                },
            new Product { Name = "A-DERMA Exomega Control Emollient Balm 200ml",
                Description = "EXOMEGA CONTROL is a complete line of products for hygiene and care of dry skin prone to atopy.",
                Category = categories[2],
                Price = 32.60m,
                QuantityInStock = 20,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/A-DERMA-Exomega-Control-Emolijentni-balzam-200ml-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[9],
                ExpiryDate = new DateTime(2025, 08, 10)
            },
            new Product {
                Name = "Humana 1800g",
                Description = "Humana 1 contains high-quality nutrients and energy and is adapted to the special nutritional needs of a newborn during the first 6 months of life.",
                Category = categories[2],
                Price = 44.90m,
                QuantityInStock = 32,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/humana-228x228.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[4],
                ExpiryDate = new DateTime(2025, 11, 05)
                },
            new Product {
                Name = "Apiglucan Immuno syrup",
                Description = "Apiglucan Imuno is a liquid food supplement that contains (1-3), (1-6)-beta-D-glucan, obtained by extraction from the yeast Saccharomyces cerevisiae, which has the best known effect on the activation of immunity, but is also used for a number of other conditions and ailments.",
                Price = 15.80m,
                QuantityInStock = 20,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/apiglukan superapoteka2-400x400.jpg",
                Category = categories[2],
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                ExpiryDate = new DateTime(2025, 11, 05)},
            new Product {
                Name = "PINO 3D Puzzle Urgent",
                Description = "The Mini 3D puzzle is a didactic tool that helps children learn to recognize shapes, practice coordination of movements, and the ability to combine different elements.",
                Category = categories[2],
                Price = 09.40m,
                QuantityInStock = 12,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/02170022-400x400.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                ExpiryDate = new DateTime(2025, 11, 07)},
            new Product {
                Name = "MAM Fruit Pacifier",
                Description = "Perfect for little beginners: with MAM pacifiers for fresh fruit and soft vegetables you can taste without fear of choking",
                Category = categories[2],
                Price = 18.90m,
                QuantityInStock = 35,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/MAM_duda_za_voce-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[18],
                ExpiryDate = new DateTime(2025, 10, 05)},
            new Product {
                Name = "Trudi Shampoo 250ml",
                Description = "Trudi shampoo with flower pollen extract is specially formulated for washing delicate and sensitive children's hair, and is suitable for everyday use.",
                Category = categories[2],
                Price = 10.10m,
                QuantityInStock = 21,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Trudi-Sampon-250-ml-Super-Apoteka-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[19],
                ExpiryDate = new DateTime(2025, 05, 05)},
            new Product {
                Name = "AVENE Sun Tinted Cream SPF50+ 50ml",
                Description = "Tinted sun protection cream SPF 50+ is intended for dry sensitive facial skin, always prone to sunburn or exposed to intense sunlight.",
                Category = categories[3],
                Price = 44.70m,
                QuantityInStock = 13,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/AVENE-SUN-Tonirana-krema-SPF50-50ml-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[7],
                ExpiryDate = new DateTime(2025, 10, 09)},
            new Product {
                Name = "Mapez spray 100ml",
                Description = "Natural protection against mosquitoes and other insects\r\n\r\nMapez spray is a mixture of non-toxic, non-irritating, effective, natural extracts suitable for use even in the youngest children from the first days of life.",
                Category = categories[3],
                Price = 16.20m,
                QuantityInStock = 16,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Mapez-sprej-protiv-komaraca-za-djecu-100ml-228x228.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[20],
                ExpiryDate = new DateTime(2025, 11, 05)},
            new Product {
                Name = "Hansaplast Aqua Protect Waterproof patch",
                Description = "Hansaplast Aqua Protect patches are waterproof and suitable for covering all types of minor wounds.Flexible, waterproof material protects when bathing and showering.",
                Category = categories[3],
                Price = 7.00m,
                QuantityInStock = 15,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/hansaplast-flaster-aqua-protect-400x400.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[21],
                ExpiryDate = new DateTime(2025, 12, 05)},
            new Product {
                Name = "Gloria Body myst 200ml",
                Description = "A soothing body mist, rich in moisturizing active substances, regenerates and refreshes the skin during hot summer days.Gives the skin a healthy look. Red algae with skin proteins create a protective layer that protects against external harmful influences, while ferulic acid has a photoprotective effect. The product is quickly absorbed and does not leave greasy traces. By pressing the pump, spray the product on the target area.",
                Category = categories[3],
                Price = 16.50m,
                QuantityInStock = 13,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Gloria-Body-mist-200-ml-Super-Apoteka-500x500.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now ,
                Brand = brands[15],
                ExpiryDate = new DateTime(2025, 08, 01)
            },
            new Product {
                Name = "Laboratorios BABE Aloe Vera gel 300ml",
                Description = "Aloe vera gel that moisturizes, soothes, softens, refreshes and restores the skin. It is especially recommended for sensitive and irritated skin.\r\nSometimes our skin becomes irritated due to situations such as prolonged exposure to the sun, shaving or waxing.",
                Category = categories[3],
                Price = 33.90m,
                QuantityInStock = 10,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Aloe-Vera-300ml-500x500.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[22],
            ExpiryDate = new DateTime(2025, 08, 01)},
            new Product {
                Name = "URIAGE Thermal water 150ml",
                Description = "Uriage thermal water for daily skin care\r\nNaturally isotonic, 100% Uriage thermal water is very rich in minerals and trace elements.\r\n100% natural and bacteriologically clean.",
                Category = categories[3],
                Price = 17.40m,
                QuantityInStock = 19,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/URIAGE-Termalna-voda-150ml-228x228.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[23],
                ExpiryDate = new DateTime(2025, 09, 01)},
            new Product {
                Name = "BEURER BY 84 Baby monitor",
                Description = "The analog baby monitor shows you your baby's mood using baby emotions. The device is suitable for every household thanks to its extremely long range of up to 800 m.",
                Category = categories[4],
                Price = 135.90m,
                QuantityInStock = 9,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/beurer-baby-monitor-BY84-500x500.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[5],
                ExpiryDate = new DateTime(2025, 10, 01)},
            new Product {
                Name = "BEURER FC 41 Pore cleaner",
                Description = "The deep pore cleaner enables deep cleaning of the pores thanks to the most modern vacuum technology.\r\nUsing round attachments, spots, blackheads and dead skin cells are effectively removed.",
                Category = categories[4],
                Price = 58.20m,
                QuantityInStock = 6,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/FC41-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[5],
                ExpiryDate = new DateTime(2025, 12, 01)},
            new Product {
                Name = "BEURER FC 65 Facial cleansing brush",
                Description = "The Beurer Pureo Deep clear facial brush enables gentle and thorough cleaning of the facial skin.\r\nThe brush cleans the facial skin by vibration or pulsation, but also improves circulation.",
                Category = categories[4],
                Price = 85.50m,
                QuantityInStock = 9,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/BEURER-FC-65-cetka-za-ciscenje-lica-0-500x500.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[5],
                ExpiryDate = new DateTime(2025, 11, 19)},
            new Product {
                Name = "MEDISANA Manicure and pedicure device MP815",
                Description = "Care and treatment of nails, cuticles and small calluses.",
                Category = categories[4],
                Price = 89.50m,
                QuantityInStock = 4,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/m815-500x500.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[3],
                ExpiryDate = new DateTime(2025, 10, 10)},
            new Product {
                Name = "BEURER LA 20 Aroma diffuser",
                Description = "The aroma diffuser for essential oils fills the entire room with pleasant scents using ultrasound technology.",
                Category = categories[4],
                Price = 78.00m,
                QuantityInStock = 7,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/beurer-aroma-difuzer-LA20-500x500.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[5],
                ExpiryDate = new DateTime(2025, 10, 10)},
            new Product {
                Name = "Elixir spray against mosquitoes and ticks 100ml",
                Description = "Protection against mosquitoes and ticks for adults and children over 2 years old.",
                Category = categories[4],
                Price = 9.10m,
                QuantityInStock = 11,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Eliksir-sprej-za-komarce-i-krpelje-100ml-400x400.png",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[24],
                ExpiryDate = new DateTime(2025, 10, 10)},
            new Product {
                Name = "BEURER MG 17 mini spa massager",
                Description = "The spa mini massager is waterproof and therefore can be used even under water, in wellness or any other occasion.",
                Category = categories[4],
                Price = 25.50m,
                QuantityInStock = 4,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Beurer-MG-17-mini-spa-masazer-228x228.jpg",
                IsDiscounted = false,
                DiscountPercentage = null,
                DatumDodavanja = DateTime.Now,
                Brand = brands[5],
                ExpiryDate = new DateTime(2025, 12, 10)},
            new Product {
                Name = "AVENE Balzam za usne 4g",
                Description = "Product for daily care of sensitive lips.\r\nFor soft, hydrated lips, whatever the circumstances.",
                Category = categories[1],
                Price = 14.90m,
                QuantityInStock = 16,
                Picture = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/AVENE-Balzam-za-usne-4g-400x400.jpg",
                IsDiscounted = true,
                DiscountPercentage = 10,
                DatumDodavanja = DateTime.Now,
                Brand = brands[7],
                ExpiryDate = new DateTime(2025, 10, 19)}
        };
        foreach (var product in products)
        {
            if (!db.Products.Any(p => p.Name == product.Name))
                db.Products.Add(product);
        }


       
        var users = new List<MyAppUser>
        {
            new MyAppUser
            {
                Username = "anaanic",
                Password = Helper.FileHelper.HashPassword("ana123"),
                FirstName = "Ana",
                LastName = "Anic",
                IsAdmin = false,
                IsCustomer = false,
                IsPharmacist = true
            },
            new MyAppUser
            {
                Username = "mujomujic",
                Password = Helper.FileHelper.HashPassword("mujo123"),
                FirstName = "Mujo",
                LastName = "Mujic",
                IsAdmin = false,
                IsCustomer = true,
                IsPharmacist = false
            },
            new MyAppUser
            {
                Username = "husohusic",
                Password = Helper.FileHelper.HashPassword("huso123"),
                FirstName = "Huso",
                LastName = "Husic",
                IsAdmin = false,
                IsCustomer = false,
                IsPharmacist = true
            },
            new MyAppUser
            {
                Username = "admin",
                Password = Helper.FileHelper.HashPassword("admin123"), // u produkciji koristi hash
                FirstName = "Admin",
                LastName = "Korisnik",
                IsAdmin = true,
                IsPharmacist = false,
                IsCustomer = false,
                Email = "admin@gmail.com"
            },
            new MyAppUser
            {
                Username = "farma",
                Password = Helper.FileHelper.HashPassword("farma123"),
                FirstName = "Farmaceut",
                LastName = "Korisnik",
                IsAdmin = false,
                IsPharmacist = true,
                IsCustomer = false,
                Email = "farma@gmail.com",
                EmploymentDate = new DateTime(2020, 5, 1),
                ProfileImageUrl = "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/picture2.png"
            },
            new MyAppUser
            {
                Username = "kupac",
                Password = Helper.FileHelper.HashPassword("kupac123"),
                FirstName = "Kupac",
                LastName = "Korisnik",
                IsAdmin = false,
                IsPharmacist = false,
                IsCustomer = true
            }
        };

        foreach (var user in users)
        {
            var existingUser = db.MyAppUsers.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null)
            {
                db.MyAppUsers.Add(user);
            }
            else
            {
                
                existingUser.Email = user.Email ?? existingUser.Email;
                existingUser.EmploymentDate = user.EmploymentDate ?? existingUser.EmploymentDate;
                existingUser.ProfileImageUrl = user.ProfileImageUrl ?? existingUser.ProfileImageUrl;
            }
        }
        await db.SaveChangesAsync(cancellationToken);
      
        var suppliers = new List<Supplier>
        {
            new Supplier { Name = "Hercegovinalijek d.o.o. Mostar", Address = "Muje Pašića 4, Mostar 88000", Phone = "036 501-500",Email ="info@hercegovinalijek.ba" },
            new Supplier { Name = "Bosnalijek d.d. Sarajevo", Address = "Jukićeva 53, Sarajevo 71000", Phone = "033 254-400",Email ="info@bosnalijek.ba" },
            new Supplier { Name = "ZADA Pharmaceuticals d.o.o. Lukavac", Address = "GH7C+R2M, M4, Bistarac Donji", Phone = "035 551-140",Email ="zada@zada.ba" },
        };
        foreach (var supplier in suppliers)
        {
            if (!db.Suppliers.Any(s => s.Name == supplier.Name))
                db.Suppliers.Add(supplier);
        }

        var advertisements = new List<Advertisement>
        {
            new Advertisement {Title ="Yasenka sinage beauty", imageURL="https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/superapoteka_web_novembar_Yasenka skinage beauty.jpg"},
            new Advertisement {Title = "Yasenka shake", imageURL="https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/superapoteka_web_novembar_Yasenka_shake.jpg"},
            new Advertisement{Title="Defendil", imageURL="https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/DEFENDIL 1.jpg"},
            new Advertisement{Title="Waya", imageURL="https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/WAYA 1.jpg"},
            new Advertisement{Title="Ducray set", imageURL="https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Ducray set 1.jpg"}
        };
        foreach (var ad in advertisements)
        {
            if (!db.Advertisements.Any(a => a.Title == ad.Title))
                db.Advertisements.Add(ad);
        }
        await db.SaveChangesAsync(cancellationToken);

        var kupacUser = await db.MyAppUsers.FirstOrDefaultAsync(u => u.Username == "kupac", cancellationToken);
        var adminUser = await db.MyAppUsers.FirstOrDefaultAsync(u => u.Username == "admin", cancellationToken);

        var bosnalijekSupplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Name.Contains("Bosnalijek"), cancellationToken);
        var hercegovinaSupplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Name.Contains("Hercegovinalijek"), cancellationToken);
        var zadaSupplier = await db.Suppliers.FirstOrDefaultAsync(s => s.Name.Contains("ZADA"), cancellationToken);

        if (kupacUser != null && adminUser != null && bosnalijekSupplier != null && hercegovinaSupplier != null && zadaSupplier != null)
        {
            var newOrders = new List<Order>
    {
         new Order
        {
            OrderDate = DateTime.Now.AddDays(-5),
            Status = "Pending",
            TotalPrice = 87.40m,
            PaymentMethod = "Credit Card",
            ShippingAddress = "Sarajevo, BiH",
            MyAppUserId = kupacUser.ID,
        },
        new Order
        {
            OrderDate = DateTime.Now.AddDays(-2),
            Status = "Shipped",
            TotalPrice = 45.99m,
            PaymentMethod = "Cash on Delivery",
            ShippingAddress = "Mostar, BiH",
            MyAppUserId = kupacUser.ID,
        },
        new Order
        {
            OrderDate = DateTime.Now,
            Status = "Delivered",
            TotalPrice = 123.75m,
            PaymentMethod = "Credit Card",
            ShippingAddress = "Zenica, BiH",
            MyAppUserId = adminUser.ID,
        },
        new Order
        {
            OrderDate = DateTime.Today,
            Status = "Delivered",
            TotalPrice = 75.90m,
            PaymentMethod = "Credit Card",
            ShippingAddress = "Sarajevo, BiH",
            MyAppUserId = adminUser.ID,
        }
    };

            foreach (var order in newOrders)
            {
                var existingOrder = await db.Orders.FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

                if (existingOrder == null)
                {
                    
                    db.Orders.Add(order);
                }
                else
                {
                    
                    existingOrder.OrderDate = order.OrderDate != DateTime.MinValue ? order.OrderDate : existingOrder.OrderDate;
                    existingOrder.Status = order.Status ?? existingOrder.Status;
                    existingOrder.TotalPrice = order.TotalPrice != 0 ? order.TotalPrice : existingOrder.TotalPrice;
                    existingOrder.MyAppUserId = order.MyAppUserId != 0 ? order.MyAppUserId : existingOrder.MyAppUserId;

                    
                    existingOrder.PaymentMethod = order.PaymentMethod ?? existingOrder.PaymentMethod;
                    existingOrder.ShippingAddress = order.ShippingAddress ?? existingOrder.ShippingAddress;
                   

                    
                    db.Orders.Update(existingOrder);
                }
            }

       
            await db.SaveChangesAsync(cancellationToken);


            
            await db.SaveChangesAsync(cancellationToken);

           
            var products1 = db.Products.ToList();
            var orders = db.Orders.ToList();

            if (products1.Count >= 3 && orders.Count >= 2)
            {
                var orderDetails = new List<OrderDetail>
        {
            new OrderDetail { OrderId = orders[0].Id, ProductId = products1[0].Id, Qty = 2, PricePerUnit = products1[0].Price },
            new OrderDetail { OrderId = orders[0].Id, ProductId = products1[1].Id, Qty = 1, PricePerUnit = products1[1].Price },
            new OrderDetail { OrderId = orders[1].Id, ProductId = products1[2].Id, Qty = 3, PricePerUnit = products1[2].Price },
            new OrderDetail { OrderId = orders[2].Id, ProductId = products1[4].Id, Qty = 3, PricePerUnit = products1[2].Price },
            new OrderDetail { OrderId = orders[3].Id, ProductId = products1[2].Id, Qty = 3, PricePerUnit = products1[2].Price },
        };

                db.OrderDetails.AddRange(orderDetails);
                await db.SaveChangesAsync(cancellationToken);

                foreach (var order in orders)
                {
                    var orderDetailItems = await db.OrderDetails
                        .Where(od => od.OrderId == order.Id)
                        .ToListAsync(cancellationToken);

                    decimal recalculatedTotal = orderDetailItems.Sum(od => od.Qty * od.PricePerUnit);

                    order.TotalPrice = recalculatedTotal;
                    db.Orders.Update(order);
                }

                await db.SaveChangesAsync(cancellationToken);
            }
            var user = db.MyAppUsers.FirstOrDefault(); 
            var orderC = db.Orders.FirstOrDefault();

            if (user != null)
            {
                var notifications = new List<Notification>
        {
            new Notification
            {
                Title = "Nova narudžba",
                Message = "Primili ste novu narudžbu za lijekove.",
                Time = DateTime.UtcNow.AddMinutes(-30),
                Read = false,
                MyAppUserId = user.ID,
                OrderId = orderC.Id
            },
            new Notification
            {
                Title = "Podsjetnik na zalihu",
                Message = "Zaliha za lijek 'Paracetamol' je ispod minimalnog nivoa.",
                Time = DateTime.UtcNow.AddHours(-2),
                Read = false,
                MyAppUserId = user.ID,
               
            },
            new Notification
            {
                Title = "Sistem",
                Message = "Vaš profil je uspješno ažuriran.",
                Time = DateTime.UtcNow.AddDays(-1),
                Read = true,
                MyAppUserId = user.ID,
                
            },
            new Notification
            {
                Title = "Nova narudžba",
                Message = "Primili ste novu narudžbu za lijekove.",
                Time = DateTime.UtcNow.AddMinutes(-30),
                Read = false,
                MyAppUserId = 5,
                OrderId = orderC.Id
            },
            new Notification
            {
                Title = "Podsjetnik na zalihu",
                Message = "Zaliha za lijek 'Paracetamol' je ispod minimalnog nivoa.",
                Time = DateTime.UtcNow.AddHours(-2),
                Read = false,
                MyAppUserId = 5,
                
            },
            new Notification
            {
                Title = "Sistem",
                Message = "Vaš profil je uspješno ažuriran.",
                Time = DateTime.UtcNow.AddDays(-1),
                Read = true,
                MyAppUserId = 5
            }
        };

                db.Notifications.AddRange(notifications);
                await db.SaveChangesAsync();
            }
        }

        return "Seeder ran successfully: New data added if it didn't already exist.";
    }
}
