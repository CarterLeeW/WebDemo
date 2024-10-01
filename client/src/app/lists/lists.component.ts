import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { MemberCardComponent } from "../members/member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [FormsModule, ButtonsModule, MemberCardComponent, PaginationModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css'
})
export class ListsComponent implements OnInit, OnDestroy{
  likesService = inject(LikesService);
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;

  ngOnInit(): void {
    this.loadLikes();
  }

  ngOnDestroy(): void {
    // ensures that signal is reloaded when the page is left
    this.likesService.paginatedResult.set(null);
  }

  getTitle(): string {
    switch (this.predicate) {
      case 'liked': return 'Members you like';
      case 'likedBy': return 'Members who like you';
      default: return 'Mutual';
    }
  }

  loadLikes(): void {
    this.likesService.getLikes(this.predicate, this.pageNumber, this.pageSize);
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
