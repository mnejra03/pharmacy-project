import { Component } from '@angular/core';
import { RecipeService } from '../../../services/recipe.service';

@Component({
  selector: 'app-recipes',
  standalone: false,
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css'
})
export class RecipesComponent {
  recipes: any[] = [];

  constructor(private recipeService: RecipeService) {}

  updateRecipeStatus(recipe: any) {
    this.recipeService.updateStatus(recipe.id, recipe.status).subscribe({
      next: () => {},
      error: err => console.error('Error updating status:', err)
    });
  }

  ngOnInit(): void {
    this.loadAllRecipes();
  }

  loadAllRecipes() {
    this.recipeService.getAllRecipes().subscribe({
      next: (data) => {
        this.recipes = data;
      },
      error: (err) => {
        console.error('Error: ', err);
      }
    });
  }
}
