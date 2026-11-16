using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BandUp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Towns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    SearchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towns", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "Rock", "rock" },
                    { 2, "Indie", "indie" },
                    { 3, "Alternative", "alternative" },
                    { 4, "Pop", "pop" },
                    { 5, "Pop punk", "pop-punk" },
                    { 6, "Punk", "punk" },
                    { 7, "Hardcore", "hardcore" },
                    { 8, "Emo", "emo" },
                    { 9, "Metal", "metal" },
                    { 10, "Grunge", "grunge" },
                    { 11, "Shoegaze", "shoegaze" },
                    { 12, "Post-rock", "post-rock" },
                    { 13, "Blues", "blues" },
                    { 14, "Jazz", "jazz" },
                    { 15, "Funk", "funk" },
                    { 16, "Soul", "soul" },
                    { 17, "R&B", "r-and-b" },
                    { 18, "Hip hop", "hip-hop" },
                    { 19, "Grime", "grime" },
                    { 20, "Drum and bass", "drum-and-bass" },
                    { 21, "Electronic", "electronic" },
                    { 22, "House", "house" },
                    { 23, "Techno", "techno" },
                    { 24, "Folk", "folk" },
                    { 25, "Country", "country" },
                    { 26, "Americana", "americana" },
                    { 27, "Singer-songwriter", "singer-songwriter" },
                    { 28, "Reggae", "reggae" },
                    { 29, "Ska", "ska" },
                    { 30, "Classical", "classical" },
                    { 31, "Gospel", "gospel" },
                    { 32, "Experimental", "experimental" },
                    { 33, "Covers", "covers" },
                    { 34, "Function / wedding band", "function-wedding-band" }
                });

            migrationBuilder.InsertData(
                table: "Instruments",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "Vocals", "vocals" },
                    { 2, "Backing vocals", "backing-vocals" },
                    { 3, "Rapper / MC", "rapper-mc" },
                    { 4, "Electric guitar", "electric-guitar" },
                    { 5, "Acoustic guitar", "acoustic-guitar" },
                    { 6, "Bass guitar", "bass-guitar" },
                    { 7, "Double bass", "double-bass" },
                    { 8, "Drums", "drums" },
                    { 9, "Percussion", "percussion" },
                    { 10, "Keyboards", "keyboards" },
                    { 11, "Piano", "piano" },
                    { 12, "Synthesiser", "synthesiser" },
                    { 13, "Organ", "organ" },
                    { 14, "DJ / Turntables", "dj-turntables" },
                    { 15, "Producer / Electronics", "producer-electronics" },
                    { 16, "Violin", "violin" },
                    { 17, "Viola", "viola" },
                    { 18, "Cello", "cello" },
                    { 19, "Saxophone", "saxophone" },
                    { 20, "Trumpet", "trumpet" },
                    { 21, "Trombone", "trombone" },
                    { 22, "Clarinet", "clarinet" },
                    { 23, "Flute", "flute" },
                    { 24, "Harmonica", "harmonica" },
                    { 25, "Banjo", "banjo" },
                    { 26, "Mandolin", "mandolin" },
                    { 27, "Ukulele", "ukulele" },
                    { 28, "Harp", "harp" },
                    { 29, "Accordion", "accordion" },
                    { 30, "Other", "other" }
                });

            migrationBuilder.InsertData(
                table: "Towns",
                columns: new[] { "Id", "Country", "Latitude", "Longitude", "Name", "Region", "SearchName" },
                values: new object[,]
                {
                    { 1, "England", 51.507199999999997, -0.12759999999999999, "London", "Greater London", "london" },
                    { 2, "England", 52.486199999999997, -1.8904000000000001, "Birmingham", "West Midlands", "birmingham" },
                    { 3, "England", 53.480800000000002, -2.2425999999999999, "Manchester", "Greater Manchester", "manchester" },
                    { 4, "England", 53.4084, -2.9916, "Liverpool", "Merseyside", "liverpool" },
                    { 5, "England", 53.800800000000002, -1.5490999999999999, "Leeds", "West Yorkshire", "leeds" },
                    { 6, "England", 53.381100000000004, -1.4701, "Sheffield", "South Yorkshire", "sheffield" },
                    { 7, "England", 51.454500000000003, -2.5878999999999999, "Bristol", "Bristol", "bristol" },
                    { 8, "England", 54.978299999999997, -1.6177999999999999, "Newcastle upon Tyne", "Tyne and Wear", "newcastle upon tyne" },
                    { 9, "England", 52.954799999999999, -1.1580999999999999, "Nottingham", "Nottinghamshire", "nottingham" },
                    { 10, "England", 52.636899999999997, -1.1397999999999999, "Leicester", "Leicestershire", "leicester" },
                    { 11, "England", 52.406799999999997, -1.5197000000000001, "Coventry", "West Midlands", "coventry" },
                    { 12, "England", 53.795999999999999, -1.7594000000000001, "Bradford", "West Yorkshire", "bradford" },
                    { 13, "England", 50.822499999999998, -0.13719999999999999, "Brighton", "East Sussex", "brighton" },
                    { 14, "England", 53.767600000000002, -0.32740000000000002, "Hull", "East Riding of Yorkshire", "hull" },
                    { 15, "England", 50.375500000000002, -4.1426999999999996, "Plymouth", "Devon", "plymouth" },
                    { 16, "England", 50.909700000000001, -1.4044000000000001, "Southampton", "Hampshire", "southampton" },
                    { 17, "England", 50.819800000000001, -1.0880000000000001, "Portsmouth", "Hampshire", "portsmouth" },
                    { 18, "England", 51.454300000000003, -0.97809999999999997, "Reading", "Berkshire", "reading" },
                    { 19, "England", 52.922499999999999, -1.4745999999999999, "Derby", "Derbyshire", "derby" },
                    { 20, "England", 53.002699999999997, -2.1793999999999998, "Stoke-on-Trent", "Staffordshire", "stoke-on-trent" },
                    { 21, "England", 52.586199999999998, -2.1288, "Wolverhampton", "West Midlands", "wolverhampton" },
                    { 22, "England", 52.630899999999997, 1.2974000000000001, "Norwich", "Norfolk", "norwich" },
                    { 23, "England", 52.205300000000001, 0.12180000000000001, "Cambridge", "Cambridgeshire", "cambridge" },
                    { 24, "England", 51.752000000000002, -1.2577, "Oxford", "Oxfordshire", "oxford" },
                    { 25, "England", 53.959000000000003, -1.0814999999999999, "York", "North Yorkshire", "york" },
                    { 26, "England", 50.718400000000003, -3.5339, "Exeter", "Devon", "exeter" },
                    { 27, "England", 51.381100000000004, -2.359, "Bath", "Somerset", "bath" },
                    { 28, "England", 52.040599999999998, -0.75939999999999996, "Milton Keynes", "Buckinghamshire", "milton keynes" },
                    { 29, "England", 52.240499999999997, -0.90269999999999995, "Northampton", "Northamptonshire", "northampton" },
                    { 30, "England", 51.878700000000002, -0.41999999999999998, "Luton", "Bedfordshire", "luton" },
                    { 31, "England", 50.719200000000001, -1.8808, "Bournemouth", "Dorset", "bournemouth" },
                    { 32, "England", 51.555799999999998, -1.7797000000000001, "Swindon", "Wiltshire", "swindon" },
                    { 33, "England", 51.864199999999997, -2.2382, "Gloucester", "Gloucestershire", "gloucester" },
                    { 34, "England", 51.8994, -2.0783, "Cheltenham", "Gloucestershire", "cheltenham" },
                    { 35, "England", 52.192, -2.2200000000000002, "Worcester", "Worcestershire", "worcester" },
                    { 36, "England", 52.0565, -2.7160000000000002, "Hereford", "Herefordshire", "hereford" },
                    { 37, "England", 52.707299999999996, -2.7553000000000001, "Shrewsbury", "Shropshire", "shrewsbury" },
                    { 38, "England", 53.193399999999997, -2.8931, "Chester", "Cheshire", "chester" },
                    { 39, "England", 53.763199999999998, -2.7031000000000001, "Preston", "Lancashire", "preston" },
                    { 40, "England", 53.817500000000003, -3.0356999999999998, "Blackpool", "Lancashire", "blackpool" },
                    { 41, "England", 54.046599999999998, -2.8007, "Lancaster", "Lancashire", "lancaster" },
                    { 42, "England", 54.892499999999998, -2.9329000000000001, "Carlisle", "Cumbria", "carlisle" },
                    { 43, "England", 54.9069, -1.3837999999999999, "Sunderland", "Tyne and Wear", "sunderland" },
                    { 44, "England", 54.574199999999998, -1.2350000000000001, "Middlesbrough", "North Yorkshire", "middlesbrough" },
                    { 45, "England", 54.7761, -1.5732999999999999, "Durham", "County Durham", "durham" },
                    { 46, "England", 53.645800000000001, -1.7849999999999999, "Huddersfield", "West Yorkshire", "huddersfield" },
                    { 47, "England", 53.683, -1.4977, "Wakefield", "West Yorkshire", "wakefield" },
                    { 48, "England", 53.522799999999997, -1.1285000000000001, "Doncaster", "South Yorkshire", "doncaster" },
                    { 49, "England", 53.230699999999999, -0.54059999999999997, "Lincoln", "Lincolnshire", "lincoln" },
                    { 50, "England", 52.056699999999999, 1.1482000000000001, "Ipswich", "Suffolk", "ipswich" },
                    { 51, "England", 51.895899999999997, 0.89190000000000003, "Colchester", "Essex", "colchester" },
                    { 52, "England", 51.735599999999998, 0.46850000000000003, "Chelmsford", "Essex", "chelmsford" },
                    { 53, "England", 51.545900000000003, 0.7077, "Southend-on-Sea", "Essex", "southend-on-sea" },
                    { 54, "England", 51.280200000000001, 1.0789, "Canterbury", "Kent", "canterbury" },
                    { 55, "England", 51.271999999999998, 0.52900000000000003, "Maidstone", "Kent", "maidstone" },
                    { 56, "England", 51.236199999999997, -0.57040000000000002, "Guildford", "Surrey", "guildford" },
                    { 57, "England", 51.109200000000001, -0.18720000000000001, "Crawley", "West Sussex", "crawley" },
                    { 58, "England", 51.656500000000001, -0.39029999999999998, "Watford", "Hertfordshire", "watford" },
                    { 59, "England", 51.752000000000002, -0.33600000000000002, "St Albans", "Hertfordshire", "st albans" },
                    { 60, "England", 52.569499999999998, -0.24049999999999999, "Peterborough", "Cambridgeshire", "peterborough" },
                    { 61, "England", 53.410600000000002, -2.1575000000000002, "Stockport", "Greater Manchester", "stockport" },
                    { 62, "England", 53.576900000000002, -2.4281999999999999, "Bolton", "Greater Manchester", "bolton" },
                    { 63, "England", 53.545000000000002, -2.6324999999999998, "Wigan", "Greater Manchester", "wigan" },
                    { 64, "England", 53.390000000000001, -2.597, "Warrington", "Cheshire", "warrington" },
                    { 65, "England", 53.487499999999997, -2.2900999999999998, "Salford", "Greater Manchester", "salford" },
                    { 66, "England", 50.263199999999998, -5.0510000000000002, "Truro", "Cornwall", "truro" },
                    { 67, "Wales", 51.4816, -3.1791, "Cardiff", "Cardiff", "cardiff" },
                    { 68, "Wales", 51.621400000000001, -3.9436, "Swansea", "Swansea", "swansea" },
                    { 69, "Wales", 51.584200000000003, -2.9977, "Newport", "Newport", "newport" },
                    { 70, "Wales", 53.046199999999999, -2.9929999999999999, "Wrexham", "Wrexham", "wrexham" },
                    { 71, "Scotland", 55.953299999999999, -3.1882999999999999, "Edinburgh", "City of Edinburgh", "edinburgh" },
                    { 72, "Scotland", 55.864199999999997, -4.2518000000000002, "Glasgow", "Glasgow City", "glasgow" },
                    { 73, "Scotland", 57.149700000000003, -2.0943000000000001, "Aberdeen", "Aberdeen City", "aberdeen" },
                    { 74, "Scotland", 56.462000000000003, -2.9706999999999999, "Dundee", "Dundee City", "dundee" },
                    { 75, "Scotland", 57.477800000000002, -4.2247000000000003, "Inverness", "Highland", "inverness" },
                    { 76, "Scotland", 56.116500000000002, -3.9369000000000001, "Stirling", "Stirling", "stirling" },
                    { 77, "Northern Ireland", 54.597299999999997, -5.9301000000000004, "Belfast", "County Antrim", "belfast" },
                    { 78, "Northern Ireland", 54.996600000000001, -7.3086000000000002, "Derry", "County Londonderry", "derry" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Slug",
                table: "Genres",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Slug",
                table: "Instruments",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Towns_Latitude_Longitude",
                table: "Towns",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Towns_SearchName",
                table: "Towns",
                column: "SearchName")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Towns");
        }
    }
}
