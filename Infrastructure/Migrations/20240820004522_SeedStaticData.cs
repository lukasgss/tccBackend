using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedStaticData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "AdoptionRestrictions",
                table: "AdoptionAlerts",
                type: "text[]",
                maxLength: 100,
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldMaxLength: 100,
                oldDefaultValue: new List<string>());

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Affenpinscher");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Galgo Afegão");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Airedale Terrier");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Akita");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Malamute do Alasca");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "SpeciesId" },
                values: new object[] { "Bulldog Americano", 1 });

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "Id", "Name", "SpeciesId" },
                values: new object[,]
                {
                    { 7, "Pit Bull Terrier Americano", 1 },
                    { 8, "Staffordshire Terrier Americano", 1 },
                    { 9, "Boiadeiro Australiano", 1 },
                    { 10, "Pastor Australiano", 1 },
                    { 11, "Terrier Australiano", 1 },
                    { 12, "Basenji", 1 },
                    { 13, "Basset Hound", 1 },
                    { 14, "Beagle", 1 },
                    { 15, "Bearded Collie", 1 },
                    { 16, "Bedlington Terrier", 1 },
                    { 17, "Boiadeiro Bernês", 1 },
                    { 18, "Bichon Frisé", 1 },
                    { 19, "Bloodhound", 1 },
                    { 20, "Border Collie", 1 },
                    { 21, "Border Terrier", 1 },
                    { 22, "Borzoi", 1 },
                    { 23, "Boston Terrier", 1 },
                    { 24, "Boxer", 1 },
                    { 25, "Spaniel Boykin", 1 },
                    { 26, "Briard", 1 },
                    { 27, "Bretão", 1 },
                    { 28, "Griffon de Bruxelas", 1 },
                    { 29, "Bull Terrier", 1 },
                    { 30, "Bulldog", 1 },
                    { 31, "Bullmastiff", 1 },
                    { 32, "Cairn Terrier", 1 },
                    { 33, "Cane Corso", 1 },
                    { 34, "Cavalier King Charles Spaniel", 1 },
                    { 35, "Retriever de Chesapeake Bay", 1 },
                    { 36, "Chihuahua", 1 },
                    { 37, "Cão de Crista Chinês", 1 },
                    { 38, "Chow Chow", 1 },
                    { 39, "Clumber Spaniel", 1 },
                    { 40, "Cocker Spaniel", 1 },
                    { 41, "Collie", 1 },
                    { 42, "Coonhound", 1 },
                    { 43, "Corgi", 1 },
                    { 44, "Coton de Tulear", 1 },
                    { 45, "Dachshund", 1 },
                    { 46, "Dálmata", 1 },
                    { 47, "Doberman Pinscher", 1 },
                    { 48, "Dogo Argentino", 1 },
                    { 49, "Dogue de Bordeaux", 1 },
                    { 50, "Bulldog Inglês", 1 },
                    { 51, "Setter Inglês", 1 },
                    { 52, "Spaniel de Springer Inglês", 1 },
                    { 53, "Retriever de Pelo Liso", 1 },
                    { 54, "Bulldog Francês", 1 },
                    { 55, "Pastor Alemão", 1 },
                    { 56, "Pointer Alemão de Pelo Curto", 1 },
                    { 57, "Golden Retriever", 1 },
                    { 58, "Dog Alemão", 1 },
                    { 59, "Cão da Serra da Estrela", 1 },
                    { 60, "Galgo Inglês", 1 },
                    { 61, "Havanês", 1 },
                    { 62, "Setter Irlandês", 1 },
                    { 63, "Lobero Irlandês", 1 },
                    { 64, "Jack Russell Terrier", 1 },
                    { 65, "Chin Japonês", 1 },
                    { 66, "Keeshond", 1 },
                    { 67, "Labrador Retriever", 1 },
                    { 68, "Lhasa Apso", 1 },
                    { 69, "Maltês", 1 },
                    { 70, "Mastim", 1 },
                    { 71, "Pinscher Miniatura", 1 },
                    { 72, "Terra Nova", 1 },
                    { 73, "Elkhound Norueguês", 1 },
                    { 74, "Papillon", 1 },
                    { 75, "Pequinês", 1 },
                    { 76, "Pembroke Welsh Corgi", 1 },
                    { 77, "Spitz Alemão", 1 },
                    { 78, "Poodle", 1 },
                    { 79, "Cão de Água Português", 1 },
                    { 80, "Pug", 1 },
                    { 81, "Rottweiler", 1 },
                    { 82, "São Bernardo", 1 },
                    { 83, "Samoyeda", 1 },
                    { 84, "Schipperke", 1 },
                    { 85, "Schnauzer", 1 },
                    { 86, "Scottish Terrier", 1 },
                    { 87, "Shiba Inu", 1 },
                    { 88, "Shih Tzu", 1 },
                    { 89, "SRD", 1 },
                    { 90, "Husky Siberiano", 1 },
                    { 91, "Staffordshire Bull Terrier", 1 },
                    { 92, "Vizsla", 1 },
                    { 93, "Weimaraner", 1 },
                    { 94, "West Highland White Terrier", 1 },
                    { 95, "Whippet", 1 },
                    { 96, "Yorkshire Terrier", 1 },
                    { 97, "Abissínio", 2 },
                    { 98, "American Curl", 2 },
                    { 99, "American Shorthair", 2 },
                    { 100, "American Wirehair", 2 },
                    { 101, "Angorá", 2 },
                    { 102, "Balinês", 2 },
                    { 103, "Bengal", 2 },
                    { 104, "Birmanês", 2 },
                    { 105, "Bobtail Americano", 2 },
                    { 106, "Bobtail Japonês", 2 },
                    { 107, "Bombay", 2 },
                    { 108, "British Shorthair", 2 },
                    { 109, "Burmês", 2 },
                    { 110, "Chartreux", 2 },
                    { 111, "Cornish Rex", 2 },
                    { 112, "Devon Rex", 2 },
                    { 113, "Egípcio Mau", 2 },
                    { 114, "Europeu", 2 },
                    { 115, "Exótico", 2 },
                    { 116, "Himalaio", 2 },
                    { 117, "Javanês", 2 },
                    { 118, "Korat", 2 },
                    { 119, "LaPerm", 2 },
                    { 120, "Maine Coon", 2 },
                    { 121, "Manx", 2 },
                    { 122, "Mau Árabe", 2 },
                    { 123, "Munchkin", 2 },
                    { 124, "Norueguês da Floresta", 2 },
                    { 125, "Ocicat", 2 },
                    { 126, "Oriental", 2 },
                    { 127, "Persa", 2 },
                    { 128, "Peterbald", 2 },
                    { 129, "Pixie-bob", 2 },
                    { 130, "Ragdoll", 2 },
                    { 131, "Russian Blue", 2 },
                    { 132, "Savannah", 2 },
                    { 133, "Scottish Fold", 2 },
                    { 134, "Selkirk Rex", 2 },
                    { 135, "Siamês", 2 },
                    { 136, "Siberiano", 2 },
                    { 137, "Singapura", 2 },
                    { 138, "Snowshoe", 2 },
                    { 139, "Somali", 2 },
                    { 140, "Sphynx", 2 },
                    { 141, "SRD", 2 },
                    { 142, "Tonkineses", 2 },
                    { 143, "Toyger", 2 },
                    { 144, "Turco Van", 2 }
                });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2,
                column: "HexCode",
                value: "#000000");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#808080", "Cinza" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "HexCode", "Name" },
                values: new object[,]
                {
                    { 4, "#A52A2A", "Marrom" },
                    { 5, "#FFD700", "Dourado" },
                    { 6, "#C0C0C0", "Prateado" },
                    { 7, "#F5F5DC", "Bege" },
                    { 8, "#D2691E", "Tigrado" },
                    { 9, "#FFFDD0", "Creme" },
                    { 10, "#FF4500", "Ruivo" },
                    { 11, "#4682B4", "Azul" },
                    { 12, "#C68E17", "Caramelo" },
                    { 13, "#7B3F00", "Chocolate" },
                    { 14, "#6B4226", "Fígado" },
                    { 15, "#C8A2C8", "Lilás" },
                    { 16, "#704214", "Sépia" },
                    { 17, "#3B3B3B", "Tigrado Preto" },
                    { 18, "#8E593C", "Zibelina" },
                    { 19, "#D2B48C", "Areia" },
                    { 20, "#FFF5EE", "Albino" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.AlterColumn<List<string>>(
                name: "AdoptionRestrictions",
                table: "AdoptionAlerts",
                type: "text[]",
                maxLength: 100,
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldMaxLength: 100,
                oldDefaultValue: new List<string>());

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Border Collie");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Pastor Alemão");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Pug");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Dachshund");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Golden");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "SpeciesId" },
                values: new object[] { "Siamês", 2 });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2,
                column: "HexCode",
                value: "#181818");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#35281E", "Marrom" });
        }
    }
}
