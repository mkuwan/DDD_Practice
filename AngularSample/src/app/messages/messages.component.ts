import { Component, OnInit } from '@angular/core';
import { MessageService } from "../message.service";

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  /**
   * messageService はテンプレート内でバインドして使用するつもりです。
   * そのため、messageService は パブリックである必要があります。
   * Angular はコンポーネント内の パブリック なプロパティのみをバインドします
   */
  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
  }

}
